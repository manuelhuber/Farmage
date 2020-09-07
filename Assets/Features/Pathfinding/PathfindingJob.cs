using Grimity.NativeCollections;
using Grimity.PathFinding;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

// ReSharper disable ForCanBeConvertedToForeach

namespace Features.Pathfinding {
[BurstCompile]
public struct PathfindingJob : IJob {
    [ReadOnly] public NativeArray<GridNode> Map;
    public int2 MapSize;
    public int2 StartPosition;
    public int2 EndPosition;

    public NativeList<PathNode> Path;
    // public NativeList<int>? visited;

    public int MoveDiagonalCost;
    public int MoveStraightCost;

    private int _startIndex;
    private int _endIndex;

    public void Execute() {
        _startIndex = CalculateIndex(StartPosition.x, StartPosition.y);
        var requestedEndIndex = CalculateIndex(EndPosition.x, EndPosition.y);
        _endIndex = FindBestEnd(requestedEndIndex, _startIndex, Map);
        var endNode = Map[_endIndex];
        var pathNodes = new NativeArray<PathNode>(Map.Length, Allocator.Temp);
        var neighbourOffsets = GetNeighbourOffsets();
        for (var index = 0; index < Map.Length; index++) {
            var gridNode = Map[index];
            pathNodes[gridNode.Index] = new PathNode {
                Index = gridNode.Index,
                HeuristicCost = HeuristicCost(gridNode.X, gridNode.Z, endNode.X, endNode.Z),
                ArrivalCost = int.MaxValue,
                TotalCost = int.MaxValue,
                CameFromNodeIndex = -1
            };
        }

        var startingNode = pathNodes[_startIndex];
        startingNode.ArrivalCost = 0;
        startingNode.UpdateTotalCost();
        pathNodes[_startIndex] = startingNode;

        // We record the closest we get to the target
        // In case the target is unreachable we can at least show the path to the closest possible node
        var bestAttemptTargetIndex = -1;
        var bestAttemptTargetDistance = int.MaxValue;

        var openList = new NativeList<int>(20, Allocator.Temp);
        var alreadyChecked = new NativeList<int>(20, Allocator.Temp);
        openList.Add(_startIndex);

        while (openList.Length != 0) {
            var currentNodeIndex = FindLowestTotalCost(openList, pathNodes);
            if (currentNodeIndex == _endIndex) break;
            var currentNode = pathNodes[currentNodeIndex];
            if (currentNode.HeuristicCost < bestAttemptTargetDistance) {
                bestAttemptTargetIndex = currentNodeIndex;
                bestAttemptTargetDistance = currentNode.HeuristicCost;
            }

            openList.Remove(currentNodeIndex);
            var neighbours = GetNeighbours(currentNodeIndex, neighbourOffsets);
            for (var i = 0; i < neighbours.Length; i++) {
                var neighbourIndex = neighbours[i];

                if (alreadyChecked.Contains(neighbourIndex) || !Map[neighbourIndex].IsWalkable) continue;

                var neighbour = pathNodes[neighbourIndex];
                var newArrivalCost =
                    currentNode.ArrivalCost + TransitionCost(currentNodeIndex, neighbourIndex);
                if (newArrivalCost >= neighbour.ArrivalCost) continue;
                neighbour.ArrivalCost = newArrivalCost;
                neighbour.UpdateTotalCost();
                neighbour.CameFromNodeIndex = currentNodeIndex;
                pathNodes[neighbour.Index] = neighbour;
                if (!openList.Contains(neighbourIndex)) openList.Add(neighbourIndex);
            }

            alreadyChecked.Add(currentNodeIndex);
        }

        // if (visited != null) {
        //     for (var index = 0; index < alreadyChecked.Length; index++) {
        //         var i = alreadyChecked[index];
        //         visited.Value.Add(i);
        //     }
        // }

        openList.Dispose();
        alreadyChecked.Dispose();
        neighbourOffsets.Dispose();

        // If we could find a path to the _endIndex node let's go to the closest alternative
        var finalEndIndex = pathNodes[_endIndex].CameFromNodeIndex != -1 ? _endIndex : bestAttemptTargetIndex;

        WritePath(pathNodes, finalEndIndex);
    }

    /// <summary>
    ///     Writes the final path to the "Path" field
    /// </summary>
    /// <param name="pathNodes">A list of all nodes</param>
    /// <param name="endIndex"></param>
    private void WritePath(NativeArray<PathNode> pathNodes, int endIndex) {
        if (endIndex == -1) return;
        var node = pathNodes[endIndex];
        while (node.CameFromNodeIndex != -1) {
            Path.Add(node);
            node = pathNodes[node.CameFromNodeIndex];
        }
    }

    private int TransitionCost(int currentIndex, int neighbourIndex) {
        var from = Map[currentIndex];
        var to = Map[neighbourIndex];
        return PathFindingUtils.ManhattanDistanceDiagonal(from.X,
            from.Z,
            to.X,
            to.Z,
            MoveStraightCost,
            MoveDiagonalCost) + to.Penalty;
    }

    private int CalculateIndex(int x, int z) {
        return x + z * MapSize.x;
    }

    private NativeList<int> GetNeighbours(int currentIndex, NativeArray<int2> neighbourOffsetArray) {
        var neighbours = new NativeList<int>(8, Allocator.Temp);
        var currentNode = Map[currentIndex];
        for (var index = 0; index < neighbourOffsetArray.Length; index++) {
            var offset = neighbourOffsetArray[index];
            var neighbourX = currentNode.X + offset.x;
            var neighbourZ = currentNode.Z + offset.y;
            if (OutOfBounds(neighbourX, neighbourZ)) continue;
            neighbours.Add(CalculateIndex(neighbourX, neighbourZ));
        }

        return neighbours;
    }

    private bool OutOfBounds(int x, int z) {
        return x < 0 || z < 0 || x >= MapSize.x || z >= MapSize.y;
    }

    private NativeArray<int2> GetNeighbourOffsets() {
        return new NativeArray<int2>(8, Allocator.Temp) {
            [0] = new int2(-1, 0), // Left
            [1] = new int2(+1, 0), // Right
            [2] = new int2(0, +1), // Up
            [3] = new int2(0, -1), // Down
            [4] = new int2(-1, -1), // Left Down
            [5] = new int2(-1, +1), // Left Up
            [6] = new int2(+1, -1), // Right Down
            [7] = new int2(+1, +1) // Right Up
        };
    }

    private int HeuristicCost(int fromX, int fromZ, int toX, int toZ) {
        return PathFindingUtils.ManhattanDistanceDiagonal(fromX,
            fromZ,
            toX,
            toZ,
            MoveStraightCost,
            MoveDiagonalCost);
    }

    private int FindLowestTotalCost(NativeList<int> openList, NativeArray<PathNode> pathNodes) {
        var bestNode = pathNodes[openList[0]];
        for (var i = 0; i < openList.Length; i++) {
            var index = openList[i];
            var node = pathNodes[index];
            if (node.TotalCost < bestNode.TotalCost
                || node.TotalCost == bestNode.TotalCost && node.HeuristicCost < bestNode.HeuristicCost
            ) {
                bestNode = node;
            }
        }

        return bestNode.Index;
    }

    /// <summary>
    ///     Finds the best valid end node for the path finding
    ///     If the requested end node is walkable returns the end node
    ///     If the requested end is not walkable it will find the best closest alternative
    ///     DOES NOT FIX ISLAND PROBLEM! If the end node is walkable but has no path this will NOT be detected here!
    /// </summary>
    /// <param name="endIndex">The index of the request path end node</param>
    /// <param name="startIndex">The index of the path starting node</param>
    /// <param name="map">A list of all nodes</param>
    /// <returns>The index of the best end node</returns>
    private int FindBestEnd(int endIndex, int startIndex, NativeArray<GridNode> map) {
        // This behaves weird on certain cases (like buildings next to walls)
        // Maybe a simple breadth-first search would be more natural
        var startNode = map[startIndex];
        var node = map[endIndex];
        while (!node.IsWalkable && node.Index != startIndex) {
            var nextX = node.X;
            var nextZ = node.Z;
            if (startNode.X < node.X) {
                nextX--;
            } else if (startNode.X > node.X) nextX++;

            if (startNode.Z < node.Z) {
                nextZ--;
            } else if (startNode.Z > node.Z) nextZ++;

            node = map[CalculateIndex(nextX, nextZ)];
        }

        return node.Index;
    }
}
}