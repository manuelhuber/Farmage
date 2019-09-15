using System;
using System.Collections.Generic;
using System.Linq;
using Grimity.Collections;
using Grimity.GameObjects;
using UnityEngine;

namespace Features.Building.Placement {
internal struct TerrainContainer {
    public List<Vector3> terrainVertices;
    public List<Vector3> terrainVerticesWorldSpace;
    public Mesh terrainMesh;
    public MeshCollider terrainCollider;
    public GameObject terrain;
}

public class Placeable : MonoBehaviour {
    public Vector3 lowerCenter;
    public LayerMask terrainLayer;
    public PlacementSettings settings;
    public bool Placable => placable;

    private MeshRenderer[] _renderer;
    private BoxCollider _floorChecker;
    private List<GameObject> _debugCubes = new List<GameObject>();

    // Currently relevant terrain that will be iterated over constantly
    private readonly Dictionary<int, TerrainContainer> _terrains = new Dictionary<int, TerrainContainer>();

    // Previously visited terrain - saved for reuse
    private readonly Dictionary<int, TerrainContainer> _terrainsArchive = new Dictionary<int, TerrainContainer>();

    // Keep this as class member to avoid recreating dict/lists every frame
    private readonly Dictionary<int, List<Tuple<int, Vector3>>> _collisionVertices =
        new Dictionary<int, List<Tuple<int, Vector3>>>();

    private bool placable;
    private int counter = 0;


    private void Start() {
        _renderer = GetComponentsInChildren<MeshRenderer>();
        var o = gameObject;
        _floorChecker = o.AddComponent<BoxCollider>();
        var bounds = Geometry.CalculateBounds(gameObject);

        // TODO remove magic numbers
        _floorChecker.center = new Vector3(0, -0.5f, 0);
        _floorChecker.isTrigger = true;
        var floorCheckerSize = bounds.size;
        floorCheckerSize.y = 20;
        floorCheckerSize.x = 2;
        floorCheckerSize.z = 2;
        _floorChecker.size = floorCheckerSize;
        lowerCenter = Geometry.LowerCenter(o);
    }

    public void FlattenFloor() {
        var collisionVertices = CollisionVertices();
        if (collisionVertices.Count == 0) {
            return;
        }

        var vertices = collisionVertices.SelectMany(pair => pair.Value).Select(tuple => tuple.Item2.y).ToList();
        var avg = vertices.Aggregate((sum, value) => sum + value) / vertices.Count;


        foreach (var entry in collisionVertices) {
            if (!_terrains.TryGetValue(entry.Key, out var terrain)) continue;
            foreach (var (index, pos) in entry.Value) {
                terrain.terrainVertices.RemoveAt(index);
                terrain.terrainVertices.Insert(index, new Vector3(pos.x, avg, pos.z));
            }

            var newHeight = Math.Abs(lowerCenter.y) + avg;
            transform.SetY(newHeight);
            terrain.terrainMesh.SetVertices(terrain.terrainVertices);
            terrain.terrainCollider.sharedMesh = null;
            terrain.terrainCollider.sharedMesh = terrain.terrainMesh;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsTerrainLayer(other)) return;
        var terrainId = other.gameObject.GetInstanceID();
        if (_terrains.ContainsKey(terrainId)) return;
        if (_terrainsArchive.ContainsKey(terrainId)) {
            _terrains.Add(terrainId, _terrainsArchive[terrainId]);
            _terrainsArchive.Remove(terrainId);
            return;
        }

        var terrainMesh = other.GetComponent<MeshFilter>().sharedMesh;
        var terrainCollider = other.GetComponent<MeshCollider>();
        var terrain = other.gameObject;
        var terrainVertices = terrainMesh.vertices.ToList();
        var localToWorld = terrain.transform.localToWorldMatrix;
        var terrainVerticesWorldSpace =
            terrainVertices.Select(vector3 => localToWorld.MultiplyPoint3x4(vector3)).ToList();
        var container = new TerrainContainer {
            terrainMesh = terrainMesh,
            terrainCollider = terrainCollider,
            terrain = terrain,
            terrainVertices = terrainVertices,
            terrainVerticesWorldSpace = terrainVerticesWorldSpace,
        };
        _terrains.Add(terrainId, container);
    }

    private bool IsTerrainLayer(Component other) {
        return terrainLayer == (terrainLayer | (1 << other.gameObject.layer));
    }

    private void OnTriggerStay(Collider other) {
        var collisionVertices = CollisionVertices();
        if (collisionVertices.Count == 0) return;
        var heights = collisionVertices.SelectMany(pair => pair.Value).Select(tuple => tuple.Item2.y).ToArray();
        var dif = heights.Max() - heights.Min();
        if (counter % 10 == 0) {
            Debug.Log($"Dif: {dif} (min: {heights.Min()}, max: {heights.Max()}");
        } else {
            counter++;
        }

        var badTerrain = dif > settings.placementThreshold;
        if (badTerrain && placable) {
            placable = false;
            foreach (var ren in _renderer) {
                ren.material = settings.placementBad;
            }
        } else if (!badTerrain && !placable) {
            placable = true;
            foreach (var ren in _renderer) {
                ren.material = settings.placementOk;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!IsTerrainLayer(other)) return;
        var id = other.gameObject.GetInstanceID();
        if (!_terrains.ContainsKey(id)) return;
        _terrainsArchive.Add(id, _terrains[id]);
        _terrains.Remove(id);
    }

    private Dictionary<int, List<Tuple<int, Vector3>>> CollisionVertices() {
        foreach (var pair in _collisionVertices) {
            pair.Value.Clear();
        }

        foreach (var entry in _terrains) {
            var container = entry.Value;
            if (container.terrainVertices == null) return _collisionVertices;
            for (var i = 0; i < container.terrainVertices.Count; i++) {
                if (!_floorChecker.bounds.Contains(container.terrainVerticesWorldSpace[i])) continue;
                var vertices = _collisionVertices.GetOrCompute(entry.Key, i1 => new List<Tuple<int, Vector3>>());
                vertices.Add(new Tuple<int, Vector3>(i, container.terrainVertices[i]));
            }
        }

        return _collisionVertices;
    }
}
}