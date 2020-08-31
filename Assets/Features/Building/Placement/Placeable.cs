using System;
using System.Collections.Generic;
using System.Linq;
using Features.Building.Structures.FieldTower;
using Features.Building.UI;
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
        {RangeCollider.RangeColliderTag, FieldTower.SphereTag, "Terrain"};

    public bool CanBePlaced => _isTerrainGood.Value && _collisions.Count == 0 && MayBePlaced.Value;
    public PlacementSettings settings;
    public LayerMask terrainLayer;
    public int2 size;

    public readonly Observable<bool> MayBePlaced = new Observable<bool>(false);

    private bool _mayBePlaced;

    private UnityEngine.Camera _camera;
    private readonly List<Collider> _collisions = new List<Collider>();
    private MeshRenderer[] _renderer = { };
    private MapManager _mapManager;
    private BoxCollider _collider;
    private readonly Observable<bool> _isTerrainGood = new Observable<bool>(false);
    private List<int2> occupiedNodes1 = new List<int2>();


    public void Init(
        PlacementSettings newSettings,
        LayerMask newTerrainLayer,
        int2 newSize) {
        settings = newSettings;
        terrainLayer = newTerrainLayer;
        size = newSize;
        _collider.center = new Vector3(0, -0.5f, 0);
        _collider.isTrigger = true;
        _collider.size = new Vector3(size.x, 1, size.y) * BuildingManager.GridSize;
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
        var gridSize = BuildingManager.GridSize;
        var nodePosition = _mapManager.WorldPositionToNode(pos);
        var newWorldPos = _mapManager.GridToWorldPosition(nodePosition.x, nodePosition.y);
        if (size.x.IsEven()) {
            newWorldPos.x -= gridSize / 2;
        }

        if (size.y.IsEven()) {
            newWorldPos.z -= gridSize / 2;
        }

        newWorldPos.y = transform.position.y;

        transform.position = newWorldPos;
        CheckTerrain();
    }

    private void CheckTerrain() {
        ClearOccupiedNodes();

        var pos = transform.position;
        var areaAroundPosition = _mapManager.GetAreaAroundPosition(pos, size);

        foreach (var node in areaAroundPosition) {
            occupiedNodes1.Add(new int2(node.X, node.Z));

            var gridNode = node;
            gridNode.Highlight = true;
            _mapManager.SetNode(node.X, node.Z, gridNode);
        }

        var goodTerrain = occupiedNodes1.All(node => _mapManager.GetNode(node.x, node.y).IsWalkable);
        _isTerrainGood.Set(goodTerrain);
    }

    private void ClearOccupiedNodes() {
        foreach (var node in occupiedNodes1) {
            var gridNode = _mapManager.GetNode(node.x, node.y);
            gridNode.Highlight = false;
            _mapManager.SetNode(node.x, node.y, gridNode);
        }

        occupiedNodes1.Clear();
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