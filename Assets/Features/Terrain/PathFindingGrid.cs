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

namespace Features.Terrain {
public class PathFindingGrid : MonoBehaviour {
    public int FindPathJobCount = 20;
    public bool Schedule;
    public bool DrawGizmos;
    public float cellSize;
    public float sizeX;
    public float sizeZ;
    public NativeArray<GridNode> map;
    public LayerMask terrainLayer;

    public int GrassPenalty {
        get => _grassPenalty;
        set {
            _grassPenalty = value;
            UpdatePenalty();
        }
    }

    public int DiagonalCost = 1;


    private int _cellCountZ;
    private int _cellCountX;
    private List<int[]> paths = new List<int[]>();
    private UnityEngine.Camera _camera;
    [SerializeField] private int _grassPenalty;


    private void Start() {
        _camera = UnityEngine.Camera.main;
        _cellCountX = Mathf.RoundToInt(sizeX / cellSize);
        _cellCountZ = Mathf.RoundToInt(sizeZ / cellSize);

        map = new NativeArray<GridNode>(_cellCountX * _cellCountZ, Allocator.Persistent);
        new Loop2D(_cellCountX, _cellCountZ).loopY((x, z) => {
            var index = x + _cellCountX * z;
            map[index] = new GridNode {
                Index = index,
                X = x,
                Z = z,
                IsWalkable = true,
                Penalty = (x == 15 || x == 16) ? 0 : GrassPenalty,
            };
        });
    }

    [Button("Update Penalty")]
    private void UpdatePenalty() {
        for (var index = 0; index < map.Length; index++) {
            var gridNode = map[index];
            gridNode.Penalty = (gridNode.X == 15 || gridNode.X == 16) ? 0 : GrassPenalty;
            map[index] = gridNode;
        }
    }

    private void OnDestroy() {
        map.Dispose();
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
            var jobHandleArray = new NativeArray<JobHandle>(FindPathJobCount, Allocator.TempJob);
            var newPath = new NativeList<FindPathJob.PathNode>[FindPathJobCount];
            for (var i = 0; i < FindPathJobCount; i++) {
                newPath[i] = new NativeList<FindPathJob.PathNode>(Allocator.TempJob);
                var findPathJob = new FindPathJob {
                    Map = map,
                    StartPosition = new int2(0, 0),
                    EndPosition = clickedNode,
                    MoveDiagonalCost = DiagonalCost,
                    MoveStraightCost = 11,
                    MapSize = new int2(_cellCountX, _cellCountZ),
                    Path = newPath[i]
                };
                if (Schedule) {
                    jobHandleArray[i] = findPathJob.Schedule();
                } else {
                    findPathJob.Execute();
                }
            }

            JobHandle.CompleteAll(jobHandleArray);
            foreach (var nativeList in newPath) {
                paths.Clear();
                paths.Add(nativeList.ToArray().Select(node => node.Index).ToArray());
                nativeList.Dispose();
            }

            jobHandleArray.Dispose();
            sw.Stop();
            Debug.Log(sw.ElapsedMilliseconds);
        }
    }

    private int2 WorldPositionToNode(Vector3 pos) {
        var transformPosition = transform.position;
        var start = transformPosition - new Vector3(sizeX / 2, 0, sizeZ / 2);
        var end = transformPosition + new Vector3(sizeX / 2, 0, sizeZ / 2);

        var percentageX = Mathf.Clamp01((pos.x - start.x - cellSize / 2) / (end.x - start.x));
        var percentageZ = Mathf.Clamp01((pos.z - start.z - cellSize / 2) / (end.z - start.z));
        return new int2(Mathf.RoundToInt(percentageX * _cellCountX), Mathf.RoundToInt(percentageZ * _cellCountZ));
    }

    private void OnDrawGizmos() {
        if (!DrawGizmos) return;
        Gizmos.DrawWireCube(transform.position, new Vector3(sizeX, 1, sizeZ));
        var size = cellSize * .9f;
        foreach (var gridNode in map) {
            if (gridNode.IsWalkable) {
                Gizmos.color = Color.green;
            }

            if (Math.Abs(gridNode.Penalty) < 0.1) {
                Gizmos.color = Color.grey;
            }

            if (paths.Any(indices => indices.Contains(gridNode.Index))) {
                Gizmos.color = Color.blue;
            }

            if (!gridNode.IsWalkable) {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawCube(
                new Vector3(gridNode.X * cellSize - sizeX / 2 + cellSize / 2,
                    0,
                    gridNode.Z * cellSize - sizeZ / 2 + cellSize / 2),
                new Vector3(size, .1f, size));
        }
    }
}
}