using System;
using System.Collections.Generic;
using System.Linq;
using Grimity.Collections;
using Grimity.GameObjects;
using MonKey.Extensions;
using UnityEngine;

namespace Features.Building.Placement {
internal struct TerrainContainer {
    public List<Vector3> TerrainVertices;
    public List<Vector3> TerrainVerticesWorldSpace;
    public Mesh TerrainMesh;
    public MeshCollider TerrainCollider;
    public GameObject Terrain;
}

public class Placeable : MonoBehaviour {
    private readonly List<Collider> _collisions = new List<Collider>();

    // Keep this as class member to avoid recreating dict/lists every frame
    private readonly Dictionary<int, List<Tuple<int, Vector3>>> _collisionVertices =
        new Dictionary<int, List<Tuple<int, Vector3>>>();

    // Currently relevant terrain that will be iterated over constantly
    private readonly Dictionary<int, TerrainContainer> _terrains = new Dictionary<int, TerrainContainer>();

    // Previously visited terrain - saved for reuse
    private readonly Dictionary<int, TerrainContainer> _terrainsArchive =
        new Dictionary<int, TerrainContainer>();

    private Bounds _bounds;
    private List<GameObject> _debugCubes = new List<GameObject>();
    private BoxCollider _floorChecker;
    private Vector3 _lastPosition = Vector3.negativeInfinity;

    private MeshRenderer[] _renderer;

    private bool _terrainIsGood = true;
    public Vector3 lowerCenter;
    public PlacementSettings settings;
    public LayerMask terrainLayer;
    public bool CanBePlaced => _terrainIsGood && _collisions.Count == 0 && CanBePayedFor;
    public bool CanBePayedFor { get; set; }

    private void Start() {
        _renderer = GetComponentsInChildren<MeshRenderer>();
        _floorChecker = gameObject.AddComponent<BoxCollider>();
        _bounds = Geometry.CalculateBounds(gameObject);
        // TODO remove magic numbers
        _floorChecker.center = new Vector3(0, -0.5f, 0);
        _floorChecker.isTrigger = true;
        var floorCheckerSize = _bounds.size;
        floorCheckerSize.y = Math.Max(floorCheckerSize.y, 5);
        // make the floor checker bigger than the building so we average the vertices around the building too
        _floorChecker.size = floorCheckerSize * 1.1f;
        lowerCenter = Geometry.LowerCenter(gameObject);
        UpdateMaterial();
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsTerrainLayer(other)) {
            if (other.gameObject.tag.Equals("AttackRangeCollider")) return;

            _collisions.Add(other);
            UpdateMaterial();
            return;
        }

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
            TerrainMesh = terrainMesh,
            TerrainCollider = terrainCollider,
            Terrain = terrain,
            TerrainVertices = terrainVertices,
            TerrainVerticesWorldSpace = terrainVerticesWorldSpace
        };
        _terrains.Add(terrainId, container);
    }

    private void OnTriggerStay(Collider other) {
        if (transform.position.Equals(_lastPosition)) return;
        _lastPosition = transform.position;
        var collisionVertices = CollisionVertices();
        if (collisionVertices.Count == 0) return;
        var heights = collisionVertices.SelectMany(pair => pair.Value)
            .Select(tuple => tuple.Item2.y)
            .ToArray();
        var dif = heights.IsEmpty() ? 0 : heights.Max() - heights.Min();

        var terrainIsGood = dif < settings.placementThreshold;
        var updateNeeded = terrainIsGood != _terrainIsGood;
        _terrainIsGood = terrainIsGood;
        if (updateNeeded) UpdateMaterial();
    }

    private void OnTriggerExit(Collider other) {
        if (!IsTerrainLayer(other)) {
            _collisions.Remove(other);
            UpdateMaterial();
            return;
        }

        var id = other.gameObject.GetInstanceID();
        if (!_terrains.ContainsKey(id)) return;
        _terrainsArchive.Add(id, _terrains[id]);
        _terrains.Remove(id);
    }

    private bool IsTerrainLayer(Component other) {
        return terrainLayer == (terrainLayer | (1 << other.gameObject.layer));
    }

    public void FlattenFloor() {
        var collisionVertices = CollisionVertices();
        if (collisionVertices.Count == 0) return;

        var vertices = collisionVertices.SelectMany(pair => pair.Value)
            .Select(tuple => tuple.Item2.y)
            .ToList();
        var avg = vertices.Aggregate((sum, value) => sum + value) / vertices.Count;


        foreach (var entry in collisionVertices) {
            if (!_terrains.TryGetValue(entry.Key, out var terrain)) continue;
            foreach (var (index, pos) in entry.Value) {
                terrain.TerrainVertices.RemoveAt(index);
                terrain.TerrainVertices.Insert(index, new Vector3(pos.x, avg, pos.z));
            }

            terrain.TerrainMesh.SetVertices(terrain.TerrainVertices);
            terrain.TerrainCollider.sharedMesh = null;
            terrain.TerrainCollider.sharedMesh = terrain.TerrainMesh;
        }
    }

    private void UpdateMaterial() {
        var material = CanBePlaced ? settings.placementOk : settings.placementBad;
        foreach (var ren in _renderer) ren.material = material;
    }

    private Dictionary<int, List<Tuple<int, Vector3>>> CollisionVertices() {
        foreach (var pair in _collisionVertices) pair.Value.Clear();

        foreach (var entry in _terrains) {
            var container = entry.Value;
            if (container.TerrainVertices == null) return _collisionVertices;
            for (var i = 0; i < container.TerrainVertices.Count; i++) {
                if (!_floorChecker.bounds.Contains(container.TerrainVerticesWorldSpace[i])) continue;
                var vertices =
                    _collisionVertices.GetOrCompute(entry.Key, i1 => new List<Tuple<int, Vector3>>());
                vertices.Add(new Tuple<int, Vector3>(i, container.TerrainVertices[i]));
            }
        }

        return _collisionVertices;
    }
}
}