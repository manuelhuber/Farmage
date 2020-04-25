using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Features.Terrain {
[BurstCompile]
public struct FindPathJob : IJob {
    [ReadOnly] public NativeArray<GridNode> Map;
    public int2 MapSize;
    public int2 StartPosition;
    public int2 EndPosition;
    public NativeList<PathNode> Path;

    private int _startIndex;
    private int _endIndex;
    public int MoveDiagonalCost;

    public int MoveStraightCost;

    public void Execute() {
        if (!Map[_endIndex].IsWalkable) return;
        var pathNodes = new NativeArray<PathNode>(Map.Length, Allocator.Temp);
        var neighbourOffsets = GetNeighbourOffsets();
        _startIndex = CalculateIndex(StartPosition.x, StartPosition.y);
        _endIndex = CalculateIndex(EndPosition.x, EndPosition.y);
        for (var index = 0; index < Map.Length; index++) {
            var gridNode = Map[index];
            pathNodes[gridNode.Index] = new PathNode {
                Index = gridNode.Index,
                HeuristicCost = HeuristicCost(gridNode.X, gridNode.Z, EndPosition.x, EndPosition.y),
                ArrivalCost = int.MaxValue,
                TotalCost = int.MaxValue,
                CameFromNodeIndex = -1
            };
        }

        var startingNode = pathNodes[_startIndex];
        startingNode.ArrivalCost = 0;
        startingNode.UpdateTotalCost();
        pathNodes[_startIndex] = startingNode;

        var openList = new NativeList<int>(20, Allocator.Temp);
        var alreadyChecked = new NativeList<int>(20, Allocator.Temp);
        openList.Add(_startIndex);

        while (openList.Length != 0) {
            var currentNodeIndex = FindLowestTotalCost(openList, pathNodes);
            if (currentNodeIndex == _endIndex) break;
            var currentNode = pathNodes[currentNodeIndex];
            openList.RemoveAtSwapBack(openList.IndexOf(currentNodeIndex));
            var neighbours = GetNeighbours(currentNodeIndex, neighbourOffsets);
            for (var i = 0; i < neighbours.Length; i++) {
                var neighbourIndex = neighbours[i];

                if (alreadyChecked.Contains(neighbourIndex) || !Map[neighbourIndex].IsWalkable) continue;

                var neighbour = pathNodes[neighbourIndex];
                var newArrivalCost = currentNode.ArrivalCost + TransitionCost(currentNodeIndex, neighbour.Index);
                if (newArrivalCost >= neighbour.ArrivalCost) continue;
                neighbour.ArrivalCost = newArrivalCost;
                neighbour.UpdateTotalCost();
                neighbour.CameFromNodeIndex = currentNodeIndex;
                pathNodes[neighbour.Index] = neighbour;
                if (!openList.Contains(neighbourIndex)) {
                    openList.Add(neighbourIndex);
                }
            }

            alreadyChecked.Add(currentNodeIndex);
        }

        openList.Dispose();
        alreadyChecked.Dispose();
        neighbourOffsets.Dispose();
        WritePath(pathNodes);
    }

    private void WritePath(NativeArray<PathNode> pathNodes) {
        if (pathNodes[_endIndex].CameFromNodeIndex == -1) return;
        var node = pathNodes[_endIndex];
        while (node.CameFromNodeIndex != -1) {
            Path.Add(node);
            node = pathNodes[node.CameFromNodeIndex];
        }
    }

    private int TransitionCost(int currentIndex, int neighbourIndex) {
        // TODO: modify cost based on something?
        var from = Map[currentIndex];
        var to = Map[neighbourIndex];
        return HeuristicCost(from.X, from.Z, to.X, to.Z);
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
        var xDistance = math.abs(fromX - toX);
        var yDistance = math.abs(fromZ - toZ);
        var remaining = math.abs(xDistance - yDistance);
        return MoveDiagonalCost * math.min(xDistance, yDistance) + MoveStraightCost * remaining;
    }

    private int FindLowestTotalCost(NativeList<int> openList, NativeArray<PathNode> pathNodes) {
        var lowestValue = int.MaxValue;
        var lowestIndex = -1;
        for (var i = 0; i < openList.Length; i++) {
            var index = openList[i];
            var node = pathNodes[index];
            if (node.TotalCost >= lowestValue) continue;
            lowestValue = node.TotalCost;
            lowestIndex = node.Index;
        }

        return lowestIndex;
    }

    public struct PathNode {
        public int Index;

        public int ArrivalCost;
        public int HeuristicCost;
        public int TotalCost;

        public int CameFromNodeIndex;

        public void UpdateTotalCost() {
            TotalCost = ArrivalCost + HeuristicCost;
        }
    }
}
}