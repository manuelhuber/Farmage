﻿/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS_Pathfinding.Scripts {
public class CodeMonkeyPathfinding : MonoBehaviour {
    private const int MoveStraightCost = 10;
    private const int MoveDiagonalCost = 14;

    private void Start() {
        // FunctionPeriodic.Create(() => {
        var startTime = Time.realtimeSinceStartup;

        var findPathJobCount = 5;
        var jobHandleArray = new NativeArray<JobHandle>(findPathJobCount, Allocator.TempJob);

        for (var i = 0; i < findPathJobCount; i++) {
            var findPathJob = new FindPathJob {
                StartPosition = new int2(0, 0),
                EndPosition = new int2(99, 99)
            };
            jobHandleArray[i] = findPathJob.Schedule();
        }

        JobHandle.CompleteAll(jobHandleArray);
        jobHandleArray.Dispose();

        Debug.Log("Time: " + (Time.realtimeSinceStartup - startTime) * 1000f);
        // }, 1f);
    }

    [BurstCompile]
    private struct FindPathJob : IJob {
        public int2 StartPosition;
        public int2 EndPosition;

        public void Execute() {
            var gridSize = new int2(100, 100);

            var pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);

            for (var x = 0; x < gridSize.x; x++)
            for (var y = 0; y < gridSize.y; y++) {
                var pathNode = new PathNode();
                pathNode.X = x;
                pathNode.Y = y;
                pathNode.Index = CalculateIndex(x, y, gridSize.x);

                pathNode.GCost = int.MaxValue;
                pathNode.HCost = CalculateDistanceCost(new int2(x, y), EndPosition);
                pathNode.CalculateFCost();

                pathNode.IsWalkable = true;
                pathNode.CameFromNodeIndex = -1;

                pathNodeArray[pathNode.Index] = pathNode;
            }

            /*
            // Place Testing Walls
            {
                PathNode walkablePathNode = pathNodeArray[CalculateIndex(1, 0, gridSize.x)];
                walkablePathNode.SetIsWalkable(false);
                pathNodeArray[CalculateIndex(1, 0, gridSize.x)] = walkablePathNode;

                walkablePathNode = pathNodeArray[CalculateIndex(1, 1, gridSize.x)];
                walkablePathNode.SetIsWalkable(false);
                pathNodeArray[CalculateIndex(1, 1, gridSize.x)] = walkablePathNode;

                walkablePathNode = pathNodeArray[CalculateIndex(1, 2, gridSize.x)];
                walkablePathNode.SetIsWalkable(false);
                pathNodeArray[CalculateIndex(1, 2, gridSize.x)] = walkablePathNode;
            }
            */

            var neighbourOffsetArray = new NativeArray<int2>(8, Allocator.Temp);
            neighbourOffsetArray[0] = new int2(-1, 0); // Left
            neighbourOffsetArray[1] = new int2(+1, 0); // Right
            neighbourOffsetArray[2] = new int2(0, +1); // Up
            neighbourOffsetArray[3] = new int2(0, -1); // Down
            neighbourOffsetArray[4] = new int2(-1, -1); // Left Down
            neighbourOffsetArray[5] = new int2(-1, +1); // Left Up
            neighbourOffsetArray[6] = new int2(+1, -1); // Right Down
            neighbourOffsetArray[7] = new int2(+1, +1); // Right Up

            var endNodeIndex = CalculateIndex(EndPosition.x, EndPosition.y, gridSize.x);

            var startNode = pathNodeArray[CalculateIndex(StartPosition.x, StartPosition.y, gridSize.x)];
            startNode.GCost = 0;
            startNode.CalculateFCost();
            pathNodeArray[startNode.Index] = startNode;

            var openList = new NativeList<int>(Allocator.Temp);
            var closedList = new NativeList<int>(Allocator.Temp);

            openList.Add(startNode.Index);

            while (openList.Length > 0) {
                var currentNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeArray);
                var currentNode = pathNodeArray[currentNodeIndex];

                if (currentNodeIndex == endNodeIndex) // Reached our destination!
                    break;

                // Remove current node from Open List
                for (var i = 0; i < openList.Length; i++)
                    if (openList[i] == currentNodeIndex) {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }

                closedList.Add(currentNodeIndex);

                for (var i = 0; i < neighbourOffsetArray.Length; i++) {
                    var neighbourOffset = neighbourOffsetArray[i];
                    var neighbourPosition =
                        new int2(currentNode.X + neighbourOffset.x, currentNode.Y + neighbourOffset.y);

                    if (!IsPositionInsideGrid(neighbourPosition, gridSize)) // Neighbour not valid position
                        continue;

                    var neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);

                    if (closedList.Contains(neighbourNodeIndex)) // Already searched this node
                        continue;

                    var neighbourNode = pathNodeArray[neighbourNodeIndex];
                    if (!neighbourNode.IsWalkable) // Not walkable
                        continue;

                    var currentNodePosition = new int2(currentNode.X, currentNode.Y);

                    var tentativeGCost =
                        currentNode.GCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                    if (tentativeGCost < neighbourNode.GCost) {
                        neighbourNode.CameFromNodeIndex = currentNodeIndex;
                        neighbourNode.GCost = tentativeGCost;
                        neighbourNode.CalculateFCost();
                        pathNodeArray[neighbourNodeIndex] = neighbourNode;

                        if (!openList.Contains(neighbourNode.Index)) openList.Add(neighbourNode.Index);
                    }
                }
            }

            var endNode = pathNodeArray[endNodeIndex];
            if (endNode.CameFromNodeIndex == -1) {
                // Didn't find a path!
                //Debug.Log("Didn't find a path!");
            } else {
                // Found a path
                var path = CalculatePath(pathNodeArray, endNode);
                /*
                foreach (int2 pathPosition in path) {
                    Debug.Log(pathPosition);
                }
                */
                path.Dispose();
            }

            pathNodeArray.Dispose();
            neighbourOffsetArray.Dispose();
            openList.Dispose();
            closedList.Dispose();
        }

        private NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode) {
            if (endNode.CameFromNodeIndex == -1) // Couldn't find a path!
                return new NativeList<int2>(Allocator.Temp);

            // Found a path
            var path = new NativeList<int2>(Allocator.Temp);
            path.Add(new int2(endNode.X, endNode.Y));

            var currentNode = endNode;
            while (currentNode.CameFromNodeIndex != -1) {
                var cameFromNode = pathNodeArray[currentNode.CameFromNodeIndex];
                path.Add(new int2(cameFromNode.X, cameFromNode.Y));
                currentNode = cameFromNode;
            }

            return path;
        }

        private bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize) {
            return
                gridPosition.x >= 0 &&
                gridPosition.y >= 0 &&
                gridPosition.x < gridSize.x &&
                gridPosition.y < gridSize.y;
        }

        private int CalculateIndex(int x, int y, int gridWidth) {
            return x + y * gridWidth;
        }

        private int CalculateDistanceCost(int2 aPosition, int2 bPosition) {
            var xDistance = math.abs(aPosition.x - bPosition.x);
            var yDistance = math.abs(aPosition.y - bPosition.y);
            var remaining = math.abs(xDistance - yDistance);
            return MoveDiagonalCost * math.min(xDistance, yDistance) + MoveStraightCost * remaining;
        }


        private int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray) {
            var lowestCostPathNode = pathNodeArray[openList[0]];
            for (var i = 1; i < openList.Length; i++) {
                var testPathNode = pathNodeArray[openList[i]];
                if (testPathNode.FCost < lowestCostPathNode.FCost) lowestCostPathNode = testPathNode;
            }

            return lowestCostPathNode.Index;
        }

        private struct PathNode {
            public int X;
            public int Y;

            public int Index;

            public int GCost;
            public int HCost;
            public int FCost;

            public bool IsWalkable;

            public int CameFromNodeIndex;

            public void CalculateFCost() {
                FCost = GCost + HCost;
            }

            public void SetIsWalkable(bool isWalkable) {
                this.IsWalkable = isWalkable;
            }
        }
    }
}
}