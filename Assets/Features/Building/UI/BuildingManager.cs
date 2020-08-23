using System;
using System.Linq;
using Features.Building.BuildMenu;
using Features.Building.Placement;
using Features.Resources;
using Grimity.Data;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Building.UI {
public struct BuildingOption {
    public BuildingMenuEntry Entry;
    public bool Buildable;
    public Action OnSelect;
}

public class BuildingManager : MonoBehaviour {
    public Grimity.Data.IObservable<BuildingOption[]> BuildingOptions => _buildingOptions;

    [SerializeField] private PlacementSettings placementSettings;
    [SerializeField] private LayerMask terrainLayer = 0;
    [SerializeField] private BuildMenu.BuildMenu buildMenu;
    [SerializeField] private int gridSize = 4;

    private bool _hasActivePlaceable;
    private Placeable _placeable;
    private ResourceManager _resourceManager;
    private BuildingMenuEntry _selected;

    private readonly Observable<BuildingOption[]> _buildingOptions =
        new Observable<BuildingOption[]>(new BuildingOption[0]);

    private void Awake() {
        _resourceManager = ResourceManager.Instance;
    }

    private void Start() {
        _resourceManager.Have.OnChange(cost => {
            UpdateBuildingOptions();
            UpdateSelectedBuilding();
        });
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            DetachFromCursor();
        }

        var clickedOnUi = EventSystem.current.IsPointerOverGameObject();
        if (_hasActivePlaceable && Input.GetMouseButtonDown(0) && !clickedOnUi)
            PlaceBuilding();
    }

    private void UpdateSelectedBuilding() {
        if (!_hasActivePlaceable) return;
        _placeable.MayBePlaced.Set(_resourceManager.CanBePayed(_selected.cost));
    }

    private void UpdateBuildingOptions() {
        _buildingOptions.Set(buildMenu.entries.Select(menuEntry => new BuildingOption {
                Entry = menuEntry, Buildable = _resourceManager.CanBePayed(menuEntry.cost),
                OnSelect = () => SelectBuilding(menuEntry)
            })
            .ToArray());
    }

    private void SelectBuilding(BuildingMenuEntry menuEntry) {
        if (_hasActivePlaceable) {
            DetachFromCursor();
        }

        _selected = menuEntry;
        AttachToCursor(menuEntry.previewPrefab,
            menuEntry.buildingPrefab.GetComponent<Structures.Building>().size);
    }

    private void AttachToCursor(GameObject prefab, int2 buildingSize) {
        var building = Instantiate(prefab);
        _placeable = building.AddComponent<Placeable>();
        _placeable.Init(placementSettings,
            terrainLayer,
            buildingSize,
            gridSize);

        _placeable.MayBePlaced.Set(true);
        _hasActivePlaceable = true;
    }

    private void PlaceBuilding() {
        if (!_placeable.CanBePlaced) {
            Debug.Log("Can't build here!");
            return;
        }

        if (_resourceManager.Pay(_selected.cost)) {
            Instantiate(_selected.buildingPrefab,
                _placeable.transform.position,
                _placeable.transform.rotation);
            if (Input.GetKey(KeyCode.LeftShift)) return;
            DetachFromCursor();
        } else {
            // TODO handle error case
            Debug.Log("Can't pay!");
        }
    }

    private void DetachFromCursor() {
        _hasActivePlaceable = false;
        if (_placeable != null) {
            Destroy(_placeable.gameObject);
        }
    }
}
}