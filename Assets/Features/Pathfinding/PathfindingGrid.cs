using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Grimity.Cursor;
using Grimity.Loops;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Features.Pathfinding {
/// <summary>
///     Testing and debuging class for pathfinding - not intended for use in game
/// </summary>
public class PathFindingGrid : MonoBehaviour {
    [SerializeField] private int grassPenalty;
    public float cellSize;
    public int diagonalCost = 1;
    public bool drawGizmos;
    public int findPathJobCount = 20;
    public bool schedule;
    public float sizeX;
    public float sizeZ;
    public LayerMask terrainLayer;

    public int GrassPenalty {
        get => grassPenalty;
        set {
            grassPenalty = value;
            UpdatePenalty();
        }
    }

    private readonly List<int[]> _paths = new List<int[]>();
    private readonly List<int> _visitedIndices = new List<int>();
    private UnityEngine.Camera _camera;
    private int _cellCountX;


    private int _cellCountZ;
    public NativeArray<GridNode> map;

    private void Start() {
        _camera = UnityEngine.Camera.main;
        _cellCountX = Mathf.RoundToInt(sizeX / cellSize);
        _cellCountZ = Mathf.RoundToInt(sizeZ / cellSize);

        map = new NativeArray<GridNode>(_cellCountX * _cellCountZ, Allocator.Persistent);
        new Loop2D(_cellCountX, _cellCountZ).LoopY((x, z) => {
            var index = x + _cellCountX * z;
            map[index] = new GridNode {
                Index = index,
                X = x,
                Z = z,
                IsWalkable = true,
                Penalty = x != 15 && x != 16 ? 0 : GrassPenalty
            };
        });
    }

    private void Update() {
        var leftClick = Input.GetMouseButton(0);
        var rightClick = Input.GetMouseButtonDown(1);
        if (!leftClick && !rightClick) return;
        CursorUtil.GetCursorLocation(out var terrainHit, _camera, terrainLayer);
        var clickedNode = WorldPositionToNode(terrainHit.point);

        if (leftClick) {
            var index = clickedNode.x + _cellCountX * clickedNode.y;
            var gridNode = map[index];
            gridNode.IsWalkable = false;
            map[index] = gridNode;
        } else if (rightClick) {
            var sw = new Stopwatch();
            sw.Start();
            var jobHandleArray = new NativeArray<JobHandle>(findPathJobCount, Allocator.TempJob);
            var newPaths = new NativeList<PathNode>[findPathJobCount];
            var visited = new NativeList<int>[findPathJobCount];
            for (var i = 0; i < findPathJobCount; i++) {
                newPaths[i] = new NativeList<PathNode>(Allocator.TempJob);
                visited[i] = new NativeList<int>(Allocator.TempJob);
                var findPathJob = new PathfindingJob {
                    Map = map,
                    StartPosition = new int2(0, 0),
                    EndPosition = clickedNode,
                    MoveDiagonalCost = diagonalCost,
                    MoveStraightCost = 11,
                    MapSize = new int2(_cellCountX, _cellCountZ),
                    Path = newPaths[i]
                    // visited = visited[i]
                };
                if (schedule) {
                    jobHandleArray[i] = findPathJob.Schedule();
                } else {
                    findPathJob.Execute();
                }
            }

            JobHandle.CompleteAll(jobHandleArray);

            _visitedIndices.Clear();
            foreach (var visitedByPath in visited) {
                foreach (var i in visitedByPath) {
                    _visitedIndices.Add(i);
                }

                visitedByPath.Dispose();
            }

            foreach (var nativeList in newPaths) {
                _paths.Clear();
                _paths.Add(nativeList.ToArray().Select(node => node.Index).ToArray());
                nativeList.Dispose();
            }

            jobHandleArray.Dispose();
            sw.Stop();
            Debug.Log("Total time: " + sw.ElapsedMilliseconds + "ms");
        }
    }

    private void OnDrawGizmos() {
        if (!drawGizmos) return;
        Gizmos.DrawWireCube(transform.position, new Vector3(sizeX, 1, sizeZ));
        var size = cellSize * .9f;
        foreach (var gridNode in map) {
            if (gridNode.IsWalkable) Gizmos.color = Color.green;

            if (Math.Abs(gridNode.Penalty) < 0.1) Gizmos.color = Color.grey;

            if (_visitedIndices.Contains(gridNode.Index)) Gizmos.color = Color.yellow;

            if (_paths.Any(indices => indices.Contains(gridNode.Index))) Gizmos.color = Color.blue;

            if (!gridNode.IsWalkable) Gizmos.color = Color.red;

            Gizmos.DrawCube(
                new Vector3(gridNode.X * cellSize - sizeX / 2 + cellSize / 2,
                    0,
                    gridNode.Z * cellSize - sizeZ / 2 + cellSize / 2),
                new Vector3(size, .1f, size));
        }
    }

    [Button("Update Penalty")]
    private void UpdatePenalty() {
        for (var index = 0; index < map.Length; index++) {
            var gridNode = map[index];
            gridNode.Penalty = gridNode.X == 15 || gridNode.X == 16 ? 0 : GrassPenalty;
            map[index] = gridNode;
        }
    }

    private int2 WorldPositionToNode(Vector3 pos) {
        var transformPosition = transform.position;
        var start = transformPosition - new Vector3(sizeX / 2, 0, sizeZ / 2);
        var end = transformPosition + new Vector3(sizeX / 2, 0, sizeZ / 2);

        var percentageX = Mathf.Clamp01((pos.x - start.x - cellSize / 2) / (end.x - start.x));
        var percentageZ = Mathf.Clamp01((pos.z - start.z - cellSize / 2) / (end.z - start.z));
        return new int2(Mathf.RoundToInt(percentageX * _cellCountX),
            Mathf.RoundToInt(percentageZ * _cellCountZ));
    }
}
}