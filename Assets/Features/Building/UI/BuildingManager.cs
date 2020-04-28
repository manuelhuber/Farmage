using System;
using System.Linq;
using Features.Building.BuildMenu;
using Features.Building.Placement;
using Features.Resources;
using Grimity.Cursor;
using Grimity.Data;
using Grimity.Math;
using UnityEngine;

namespace Features.Building.UI {
public struct BuildingOption {
    public BuildingMenuEntry Entry;
    public bool Buildable;
    public Action OnSelect;
}

public class BuildingManager : MonoBehaviour {
    public Grimity.Data.IObservable<BuildingOption[]> BuildingOptions => _buildingOptions;

    [SerializeField] private PlacementSettings placementSettings = null;
    [SerializeField] private LayerMask terrainLayer = 0;
    [SerializeField] private BuildMenu.BuildMenu buildMenu = null;
    [SerializeField] private int gridSize = 4;

    private UnityEngine.Camera _camera;
    private bool _dragObject;
    private Placeable _placeable;
    private ResourceManager _resourceManager;
    private BuildingMenuEntry _selected;

    private readonly Observable<BuildingOption[]> _buildingOptions =
        new Observable<BuildingOption[]>(new BuildingOption[0]);

    private void Awake() {
        _resourceManager = ResourceManager.Instance;
        _camera = GetComponent<UnityEngine.Camera>();
        if (_camera == null) _camera = UnityEngine.Camera.main;
    }

    private void Start() {
        _resourceManager.Have.OnChange(cost => {
            UpdateBuildingOptions();
            UpdateSelectedBuilding();
        });
    }

    private void Update() {
        if (_dragObject) {
            var pos = MouseToTerrain().point;
            var size = _selected.buildingPrefab.GetComponent<Structures.Building>().size;
            pos.x = MathUtils.RoundToMultiple(pos.x, gridSize, size.x.IsEven());
            pos.z = MathUtils.RoundToMultiple(pos.z, gridSize, size.y.IsEven());
            _placeable.transform.position = pos;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            _dragObject = false;
            if (_placeable != null) {
                Destroy(_placeable.gameObject);
                _placeable = null;
            }
        }

        if (Input.GetMouseButtonDown(0) && _dragObject) PlaceBuilding();
    }

    private void UpdateSelectedBuilding() {
        if (!_dragObject) return;
        _placeable.CanBePayedFor = _resourceManager.CanBePayed(_selected.cost);
    }

    private void UpdateBuildingOptions() {
        _buildingOptions.Set(buildMenu.entries.Select(menuEntry => new BuildingOption {
                Entry = menuEntry, Buildable = _resourceManager.CanBePayed(menuEntry.cost),
                OnSelect = () => SelectBuilding(menuEntry)
            })
            .ToArray());
    }

    private void SelectBuilding(BuildingMenuEntry menuEntry) {
        if (_dragObject) {
            DettachFromCursor();
        }

        _selected = menuEntry;
        AttachToCursor(menuEntry.previewPrefab);
    }

    private void AttachToCursor(GameObject prefab) {
        var building = Instantiate(prefab);
        _placeable = building.AddComponent<Placeable>();
        _placeable.CanBePayedFor = true;
        _placeable.terrainLayer = terrainLayer;
        _placeable.settings = placementSettings;
        _dragObject = true;
    }

    private void PlaceBuilding() {
        if (!_placeable.CanBePlaced) return;
        if (_resourceManager.Pay(_selected.cost)) {
            // _placeable.FlattenFloor();
            Instantiate(_selected.buildingPrefab,
                _placeable.transform.position,
                _placeable.transform.rotation);
            if (Input.GetKey(KeyCode.LeftShift)) return;
            DettachFromCursor();
        } else {
            // TODO handle error case
            Debug.Log("Can't pay!");
        }
    }

    private void DettachFromCursor() {
        _dragObject = false;
        Destroy(_placeable.gameObject);
    }

    private RaycastHit MouseToTerrain() {
        CursorUtil.GetCursorLocation(out var terrainHit, _camera, terrainLayer);
        return terrainHit;
    }
}
}