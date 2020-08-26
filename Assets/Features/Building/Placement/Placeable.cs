using System;
using System.Collections.Generic;
using System.Linq;
using Features.Building.Structures.FieldTower;
using Features.Common;
using Features.Pathfinding;
using Grimity.Cursor;
using Grimity.Data;
using Grimity.Math;
using Unity.Mathematics;
using UnityEngine;

namespace Features.Building.Placement {
public class Placeable : MonoBehaviour {
    private static readonly string[] IgnoredColliderTags =
        {RangeCollider.RangeColliderTag, FieldTower.SphereTag};

    public bool CanBePlaced => _isTerrainGood.Value && _collisions.Count == 0 && MayBePlaced.Value;
    public PlacementSettings settings;
    public LayerMask terrainLayer;
    public int2 size;
    public int gridSize;

    public readonly Observable<bool> MayBePlaced = new Observable<bool>(false);

    private bool _mayBePlaced;

    private UnityEngine.Camera _camera;
    private readonly List<Collider> _collisions = new List<Collider>();
    private MeshRenderer[] _renderer = { };
    private MapManager _mapManager;
    private BoxCollider _collider;
    private readonly Observable<bool> _isTerrainGood = new Observable<bool>(false);

    public void Init(
        PlacementSettings newSettings,
        LayerMask newTerrainLayer,
        int2 newSize,
        int newGridSize) {
        settings = newSettings;
        terrainLayer = newTerrainLayer;
        size = newSize;
        gridSize = newGridSize;
        _collider.center = new Vector3(0, -0.5f, 0);
        _collider.isTrigger = true;
        _collider.size = new Vector3(size.x, 1, size.y) * gridSize;
    }

    private void Awake() {
        _renderer = GetComponentsInChildren<MeshRenderer>();
        _camera = UnityEngine.Camera.main;
        _mapManager = MapManager.Instance;
        _isTerrainGood.OnChange(b => { UpdateMaterial(); }, false);
        MayBePlaced.OnChange(b => { UpdateMaterial(); }, false);
        _collider = gameObject.AddComponent<BoxCollider>();
    }

    private void Start() {
        UpdateMaterial();
    }

    private void Update() {
        var pos = MouseToTerrain().point;
        pos.x = MathUtils.RoundToMultiple(pos.x, gridSize, size.x.IsEven());
        pos.z = MathUtils.RoundToMultiple(pos.z, gridSize, size.y.IsEven());
        transform.position = pos;
        CheckTerrain();
    }

    private void CheckTerrain() {
        var pos = transform.position;
        var center = _mapManager.WorldPositionToNode(pos);
        var occupiedNodes = new List<int2>();

        var xOffsetMax = (int) Math.Floor((double) (size.x / 2));
        var yOffsetMax = (int) Math.Floor((double) (size.y / 2));
        for (var xOffset = -xOffsetMax; xOffset <= xOffsetMax; xOffset++) {
            for (var yOffset = -yOffsetMax; yOffset <= yOffsetMax; yOffset++) {
                occupiedNodes.Add(new int2(center.x + xOffset, center.y + yOffset));
            }
        }

        var goodTerrain = occupiedNodes.All(node => _mapManager.GetNode(node.x, node.y).IsWalkable);
        _isTerrainGood.Set(goodTerrain);
    }

    private void OnTriggerEnter(Collider other) {
        if (IsTerrainLayer(other)) return;
        if (IgnoredColliderTags.Contains(other.gameObject.tag)) return;
        _collisions.Add(other);
        UpdateMaterial();
    }

    private void OnTriggerExit(Collider other) {
        if (IsTerrainLayer(other)) return;
        _collisions.Remove(other);
        UpdateMaterial();
    }

    private bool IsTerrainLayer(Component other) {
        return terrainLayer == (terrainLayer | (1 << other.gameObject.layer));
    }

    private void UpdateMaterial() {
        var material = CanBePlaced ? settings.placementOk : settings.placementBad;
        foreach (var ren in _renderer) ren.material = material;
    }

    private RaycastHit MouseToTerrain() {
        CursorUtil.GetCursorLocation(out var terrainHit, _camera, terrainLayer);
        return terrainHit;
    }
}
}