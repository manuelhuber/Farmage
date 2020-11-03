using System;
using System.Collections.Generic;
using System.Linq;
using Features.Building.BuildMenu;
using Features.Building.Placement;
using Features.Resources;
using Features.Tasks;
using Features.Ui.Actions;
using Features.Ui.UserInput;
using Grimity.Data;
using Grimity.Singleton;
using UnityEngine;

namespace Features.Building.UI {
public class BuildingManager : GrimitySingleton<BuildingManager>, IInputReceiver {
    public static readonly int GridSize = 4;

    [SerializeField] private PlacementSettings placementSettings;
    [SerializeField] private GameObject constructionSitePrefab;
    [SerializeField] private LayerMask terrainLayer = 0;
    [SerializeField] private BuildMenu.BuildMenu buildMenu;
    public Grimity.Data.IObservable<ActionEntry[]> BuildingOptions => _buildingOptions;

    private readonly Observable<ActionEntry[]> _buildingOptions =
        new Observable<ActionEntry[]>(new ActionEntry[0]);

    private bool _hasActivePlaceable;
    private InputManager _inputManager;
    private Placeable _placeable;
    private ResourceManager _resourceManager;
    private BuildingMenuEntry _selected;
    private TaskManager _taskManager;

    private void Awake() {
        _inputManager = InputManager.Instance;
        _taskManager = TaskManager.Instance;
        _resourceManager = ResourceManager.Instance;
    }

    private void Start() {
        _resourceManager.Have.OnChange(cost => {
            // TODO: It's not necessary to rebuild entire options here - would be enough to just update the
            // Active property based on available resources
            UpdateBuildingOptions();
            UpdateSelectedBuilding();
        });
    }

    #region InputReceiver

    public event EventHandler YieldControl;

    public void OnKeyDown(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
    }

    public void OnKeyUp(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
        if (keys.Contains(KeyCode.Escape)) {
            FinishBuilding();
        }

        if (keys.Contains(KeyCode.Mouse0) && _hasActivePlaceable) {
            var createdBuilding = PlaceBuilding();
            var continueBuilding = keys.Contains(KeyCode.LeftShift);
            if (createdBuilding && !continueBuilding) {
                FinishBuilding();
            }
        }
    }

    public void OnKeyPressed(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
    }

    #endregion

    private void UpdateSelectedBuilding() {
        if (!_hasActivePlaceable) return;
        _placeable.MayBePlaced.Set(_resourceManager.CanBePayed(_selected.cost));
    }

    private void UpdateBuildingOptions() {
        _buildingOptions.Set(buildMenu.entries.Select(menuEntry => new ActionEntry {
                Image = menuEntry.image, Active = _resourceManager.CanBePayed(menuEntry.cost),
                OnSelect = () => SelectBuilding(menuEntry)
            })
            .ToArray());
    }

    private void SelectBuilding(BuildingMenuEntry menuEntry) {
        _inputManager.RequestControl(this);

        if (_hasActivePlaceable) {
            RemoveActivePlaceable();
        }

        _selected = menuEntry;
        AttachSelectionToCursor();
    }

    private void AttachSelectionToCursor() {
        var preview = Instantiate(_selected.modelPrefab);
        _placeable = preview.AddComponent<Placeable>();
        _placeable.Init(placementSettings, terrainLayer, _selected.size);
        _placeable.MayBePlaced.Set(true);
        _hasActivePlaceable = true;
    }

    private bool PlaceBuilding() {
        if (!_placeable.CanBePlaced) {
            Debug.Log("Can't build here!");
            return false;
        }

        if (!_resourceManager.Pay(_selected.cost)) {
            Debug.Log("Can't pay!");
            return false;
        }

        _placeable.OccupyTerrain();
        CreateConstructionSite();
        return true;
    }

    private void CreateConstructionSite() {
        var location = _placeable.transform;
        var constructionSiteGameObject =
            Instantiate(constructionSitePrefab, location.position, location.rotation);
        var constructionSite = constructionSiteGameObject.GetComponent<Construction.Construction>();
        constructionSite.Init(_selected);

        _taskManager.Enqueue(new SimpleTask(constructionSiteGameObject, TaskType.Build));
    }

    private void FinishBuilding() {
        RemoveActivePlaceable();
        YieldControl?.Invoke(this, EventArgs.Empty);
    }

    private void RemoveActivePlaceable() {
        _hasActivePlaceable = false;
        if (_placeable != null) {
            Destroy(_placeable.gameObject);
        }
    }
}
}