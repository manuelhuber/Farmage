using System;
using System.Linq;
using Features.Building.BuildMenu;
using Features.Building.Construction;
using Features.Building.Placement;
using Features.Health;
using Features.Queue;
using Features.Resources;
using Grimity.Data;
using Grimity.GameObjects;
using Ludiq.PeekCore;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using Task = Features.Queue.Task;

namespace Features.Building.UI {
public struct BuildingOption {
    public BuildingMenuEntry Entry;
    public bool Buildable;
    public Action OnSelect;
}

public class BuildingManager : MonoBehaviour {
    public Grimity.Data.IObservable<BuildingOption[]> BuildingOptions => _buildingOptions;

    [SerializeField] private JobMultiQueue farmerQueue;
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
        AttachSelectionToCursor();
    }

    private void AttachSelectionToCursor() {
        var buildingSize = _selected.buildingPrefab.GetComponent<Structures.Building>().size;
        var building = Instantiate(_selected.previewPrefab);
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
            CreateConstructionSite();
        } else {
            // TODO handle error case
            Debug.Log("Can't pay!");
        }
    }

    private void CreateConstructionSite() {
        var constructionSite = _placeable.gameObject.AddComponent<ConstructionSite>();
        constructionSite.progressTarget = _selected.buildingPrefab.GetComponent<Mortal>().MaxHitpoints;
        constructionSite.finalBuildingPrefab = _selected.buildingPrefab;
        farmerQueue.Enqueue(new Task {type = TaskType.Build, payload = constructionSite.gameObject});
        Destroy(_placeable);
        _placeable = null;
        _hasActivePlaceable = false;
    }

    private void DetachFromCursor() {
        _hasActivePlaceable = false;
        if (_placeable != null) {
            Destroy(_placeable.gameObject);
        }
    }
}
}