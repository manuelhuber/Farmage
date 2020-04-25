using System.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Features.Terrain {
[BurstCompile]
public struct FindPathJob : IJob {
    [ReadOnly] public NativeArray<GridNode> Map;
    public int2 MapSize;
    public int2 StartPosition;
    public int2 EndPosition;

    private int _startIndex;
    private int _endIndex;
    public int MoveDiagonalCost;

    public int MoveStraightCost;

    public void Execute() {
        var pathNodes = new NativeArray<PathNode>(Map.Length, Allocator.Temp);
        var neighbourOffsets = GetNeighbourOffsets();
        _startIndex = CalculateIndex(StartPosition.x, StartPosition.y);
        _endIndex = CalculateIndex(EndPosition.x, EndPosition.y);

        for (var index = 0; index < Map.Length; index++) {
            var gridNode = Map[index];
            pathNodes[gridNode.index] = new PathNode {
                index = gridNode.index,
                heuristicCost = HeuristicCost(gridNode.x, gridNode.z, Map[gridNode.index].x, Map[gridNode.index].z),
                arrivalCost = int.MaxValue,
                totalCost = int.MaxValue,
                cameFromNodeIndex = -1
            };
        }

        var startingNode = pathNodes[_startIndex];
        startingNode.arrivalCost = 0;
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
            for (var index = 0; index < neighbours.Length; index++) {
                var neighbourIndex = neighbours[index];
                if (alreadyChecked.Contains(neighbourIndex)) continue;
                var neighbour = pathNodes[neighbourIndex];
                if (!Map[neighbourIndex].isWalkable) continue;
                var newArrivalCost = currentNode.arrivalCost + TransitionCost(currentNodeIndex, neighbour.index);
                if (newArrivalCost >= neighbour.arrivalCost) continue;
                neighbour.arrivalCost = newArrivalCost;
                neighbour.UpdateTotalCost();
                neighbour.cameFromNodeIndex = currentNodeIndex;
                pathNodes[neighbour.index] = neighbour;
                if (!openList.Contains(neighbourIndex)) {
                    openList.Add(neighbourIndex);
                }
            }

            alreadyChecked.Add(currentNodeIndex);
        }

        if (pathNodes[_endIndex].cameFromNodeIndex != -1) {
            Debug.Log("Found path");
            var node = pathNodes[_endIndex];

            while (node.cameFromNodeIndex != -1) {
                Debug.Log("Node");
                Debug.Log(Map[node.index].x);
                Debug.Log(Map[node.index].z);
                node = pathNodes[node.cameFromNodeIndex];
            }
        } else {
            Debug.Log("Found no path");
        }

        openList.Dispose();
        alreadyChecked.Dispose();
        neighbourOffsets.Dispose();
        neighbourOffsets.Dispose();
    }

    private int TransitionCost(int currentIndex, int neighbhourIndex) {
        // TODO: modify cost based on something?
        var from = Map[currentIndex];
        var to = Map[neighbhourIndex];
        return HeuristicCost(from.x, from.z, to.x, to.z);
    }

    private int CalculateIndex(int x, int z) {
        // TODO maybe mapSize.y?
        return x + z * MapSize.x;
    }

    private NativeList<int> GetNeighbours(int currentIndex, NativeArray<int2> neighbourOffsetArray) {
        var neighbours = new NativeList<int>(8, Allocator.Temp);
        var currentNode = Map[currentIndex];
        foreach (var offset in neighbourOffsetArray) {
            var neighbourX = currentNode.x + offset.x;
            var neighbourZ = currentNode.z + offset.y;
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

    private int FindLowestTotalCost(NativeList<int> openList, NativeArray<PathNode> _pathNodes) {
        var lowestValue = int.MaxValue;
        var lowestIndex = -1;
        foreach (var index in openList) {
            var node = _pathNodes[index];
            if (node.totalCost >= lowestValue) continue;
            lowestValue = node.totalCost;
            lowestIndex = node.index;
        }

        return lowestIndex;
    }

    private struct PathNode {
        public int index;

        public int arrivalCost;
        public int heuristicCost;
        public int totalCost;

        public int cameFromNodeIndex;

        public void UpdateTotalCost() {
            totalCost = arrivalCost + heuristicCost;
        }
    }
}
}