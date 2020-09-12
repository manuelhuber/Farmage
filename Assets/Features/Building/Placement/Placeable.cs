using System.Collections.Generic;
using System.Linq;
using Constants;
using Features.Building.UI;
using Features.Pathfinding;
using Grimity.Cursor;
using Grimity.Data;
using Grimity.Math;
using Unity.Mathematics;
using UnityEngine;

namespace Features.Building.Placement {
public class Placeable : MonoBehaviour {
    // We ignore "Buildings" to make sure we can place buildings adjacent to each other
    // Building inside other buildings is prevented since their map tiles are marked as
    // not walkable
    private static readonly string[] IgnoredColliderTags =
        {Tags.RangeColliderTag, Tags.SphereTag, Tags.Terrain, Tags.Building};

    public bool CanBePlaced => _isTerrainGood.Value && _collisions.Count == 0 && MayBePlaced.Value;
    private readonly List<Collider> _collisions = new List<Collider>();
    private readonly Observable<bool> _isTerrainGood = new Observable<bool>(false);
    private readonly List<int2> _occupiedNodes = new List<int2>();
    public readonly Observable<bool> MayBePlaced = new Observable<bool>(false);
    private UnityEngine.Camera _camera;
    private BoxCollider _collider;
    private MapManager _mapManager;
    private MeshRenderer[] _renderer = { };

    private PlacementSettings _settings;
    private int2 _size;
    private LayerMask _terrainLayer;


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
        if (_size.x.IsEven()) {
            newWorldPos.x -= gridSize / 2;
        }

        if (_size.y.IsEven()) {
            newWorldPos.z -= gridSize / 2;
        }

        newWorldPos.y = transform.position.y;

        transform.position = newWorldPos;
        CheckTerrain();
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


    public void Init(
        PlacementSettings newSettings,
        LayerMask newTerrainLayer,
        int2 newSize) {
        _settings = newSettings;
        _terrainLayer = newTerrainLayer;
        _size = newSize;
        _collider.center = new Vector3(0, -0.5f, 0);
        _collider.isTrigger = true;
        _collider.size = new Vector3(_size.x, 1, _size.y) * BuildingManager.GridSize;
    }

    public void OccupyTerrain() {
        foreach (var node in _occupiedNodes) {
            var gridNode = _mapManager.GetNode(node.x, node.y);
            gridNode.IsWalkable = false;
            _mapManager.SetNode(node.y, node.x, gridNode);
        }
    }

    private void CheckTerrain() {
        ClearOccupiedNodes();

        var pos = transform.position;
        var areaAroundPosition = _mapManager.GetAreaAroundPosition(pos, _size);

        foreach (var node in areaAroundPosition) {
            _occupiedNodes.Add(new int2(node.X, node.Z));

            var gridNode = node;
            gridNode.Highlight = true;
            _mapManager.SetNode(node.Z, node.X, gridNode);
        }

        var goodTerrain = _occupiedNodes.All(node => _mapManager.GetNode(node.x, node.y).IsWalkable);
        _isTerrainGood.Set(goodTerrain);
    }

    private void ClearOccupiedNodes() {
        foreach (var node in _occupiedNodes) {
            var gridNode = _mapManager.GetNode(node.x, node.y);
            gridNode.Highlight = false;
            _mapManager.SetNode(node.y, node.x, gridNode);
        }

        _occupiedNodes.Clear();
    }

    private bool IsTerrainLayer(Component other) {
        return _terrainLayer == (_terrainLayer | (1 << other.gameObject.layer));
    }

    private void UpdateMaterial() {
        var material = CanBePlaced ? _settings.placementOk : _settings.placementBad;
        foreach (var ren in _renderer) {
            ren.material = material;
        }
    }

    private RaycastHit MouseToTerrain() {
        CursorUtil.GetCursorLocation(out var terrainHit, _camera, _terrainLayer);
        return terrainHit;
    }
}
}