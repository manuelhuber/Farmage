using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grimity.Layer;
using Grimity.Loops;
using Grimity.NativeCollections;
using Grimity.Singleton;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Features.Pathfinding {
public struct PathRequest {
    public Vector3 From;
    public Vector3 To;
    public Transform Movement;
    public Action<Vector3[]> Callback;
}

public class MapManager : GrimitySingleton<MapManager> {
    private readonly Queue<PathRequest> _requests = new Queue<PathRequest>();
    private int _cellCountX;
    private int _cellCountZ;
    private NativeArray<GridNode> _map;

    public int nodeScansPerFrame;
    public float cellSize;
    public float sizeX;
    public float sizeZ;

    [InfoBox("Layers that block pathfinding")]
    public LayerMask blockingLayer;

    [InfoBox("All layers (incl blocking) that count as terrain")]
    public LayerMask terrainLayer;

    private int2 _mapSize;
    private List<PathRequest> _activeRequests = new List<PathRequest>();

    private void Start() {
        _cellCountX = Mathf.RoundToInt(sizeX / cellSize);
        _cellCountZ = Mathf.RoundToInt(sizeZ / cellSize);
        _mapSize = new int2(_cellCountX, _cellCountZ);

        _map = new NativeArray<GridNode>(_cellCountX * _cellCountZ, Allocator.Persistent);
        new Loop2D(_cellCountX, _cellCountZ).LoopY((x, z) => {
            var index = x + _cellCountX * z;
            _map[index] = new GridNode {
                Index = index,
                X = x,
                Z = z,
                IsWalkable = true,
                Penalty = 0
            };
        });
        StartCoroutine(ScanMap());
    }

    private void OnDestroy() {
        _map.Dispose();
    }

    public Action RequestPath(PathRequest request) {
        _requests.Enqueue(request);
        return () => {
            if (!_activeRequests.Remove(request)) {
                Debug.Log("Canceled requests before it got calculated");
            }
        };
    }

    private void Update() {
        var requestsCount = _requests.Count;
        if (requestsCount == 0) return;
        var jobHandleArray = new NativeArray<JobHandle>(requestsCount, Allocator.TempJob);
        var newPaths = new NativeList<PathNode>[requestsCount];
        var callbacks = new Action<Vector3[]>[requestsCount];

        Debug.Log($"Scheduling {requestsCount} pathFindingJobs");
        for (var i = 0; i < requestsCount; i++) {
            var pathRequest = _requests.Dequeue();
            newPaths[i] = new NativeList<PathNode>(Allocator.TempJob);
            jobHandleArray[i] = new PathfindingJob {
                Map = _map,
                StartPosition = WorldPositionToNode(pathRequest.From),
                EndPosition = WorldPositionToNode(pathRequest.To),
                MoveDiagonalCost = 14,
                MoveStraightCost = 11,
                MapSize = _mapSize,
                Path = newPaths[i],
            }.Schedule();
            callbacks[i] = pathRequest.Callback;
            _activeRequests.Add(pathRequest);
        }

        JobHandle.CompleteAll(jobHandleArray);

        for (var i = 0; i < requestsCount; i++) {
            var gridPath = newPaths[i];
            var worldPath = gridPath.ToArray()
                .Select(node => _map[node.Index])
                .Select(node => GridToWorldPosition(node.X, node.Z))
                .ToArray();
            callbacks[i].Invoke(worldPath);
            gridPath.Dispose();
        }

        jobHandleArray.Dispose();
    }

    private IEnumerator ScanMap() {
        while (true) {
            var count = 0;
            for (var x = 0; x < _cellCountX; x++)
            for (var z = 0; z < _cellCountZ; z++) {
                count++;
                UpdateGridNode(x, z);
                if (count % nodeScansPerFrame != 0) continue;
                yield return null;
            }
        }
    }

    private void UpdateGridNode(int x, int z) {
        var origin = GridToWorldPosition(x, z);
        origin.y = 50;
        var ray = new Ray(origin, Vector3.down * 100);
        if (!Physics.Raycast(ray, out var hit, 100, terrainLayer)) return;
        var gridNode = _map.Get2D(z, x, _cellCountX);
        gridNode.IsWalkable = !blockingLayer.Contains(hit.collider.gameObject.layer);
        _map.Put2D(gridNode, z, x, _cellCountX);
    }

    private Vector3 GridToWorldPosition(int x, int z) {
        return new Vector3(x * cellSize - sizeX / 2 + cellSize / 2,
            0,
            z * cellSize - sizeZ / 2 + cellSize / 2);
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

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(sizeX, 1, sizeZ));
        var size = cellSize * .9f;
        foreach (var gridNode in _map.Where(gridNode => !gridNode.IsWalkable)) {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(
                GridToWorldPosition(gridNode.X, gridNode.Z),
                new Vector3(size, .1f, size));
        }
    }
}
}