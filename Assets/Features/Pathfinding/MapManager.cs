using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Features.Terrain;
using Grimity.Layer;
using Grimity.Loops;
using Grimity.NativeCollections;
using Grimity.Singleton;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Features.Pathfinding {
public struct PathRequest {
    public Vector3 From;
    public Vector3 To;
    public Action<Vector3[]> Callback;
}

public class MapManager : GrimitySingleton<MapManager> {
    private int _cellCountX;

    private int _cellCountZ;
    private readonly Queue<PathRequest> _requests = new Queue<PathRequest>();

    [InfoBox("Layers that block pathfinding")]
    public LayerMask blockingLayer;

    public float cellSize;
    public NativeArray<GridNode> map;

    public int nodeScansPerFrame;
    public float sizeX;
    public float sizeZ;

    [InfoBox("All layers (incl blocking) that count as terrain")]
    public LayerMask terrainLayer;

    private void Start() {
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
                Penalty = 0
            };
        });
        StartCoroutine(ScanMap());
    }

    private void OnDestroy() {
        map.Dispose();
    }

    private void Update() {
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
        var gridNode = map.Get2D(z, x, _cellCountX);
        gridNode.IsWalkable = !blockingLayer.Contains(hit.collider.gameObject.layer);
        map.Put2D(gridNode, z, x, _cellCountX);
    }

    public void RequestPath(PathRequest request) {
        _requests.Enqueue(request);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(sizeX, 1, sizeZ));
        var size = cellSize * .9f;
        foreach (var gridNode in map.Where(gridNode => !gridNode.IsWalkable)) {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(
                GridToWorldPosition(gridNode.X, gridNode.Z),
                new Vector3(size, .1f, size));
        }
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
        return new int2(Mathf.RoundToInt(percentageX * _cellCountX), Mathf.RoundToInt(percentageZ * _cellCountZ));
    }
}
}