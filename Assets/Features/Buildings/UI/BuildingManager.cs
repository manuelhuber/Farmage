using System.Collections.Generic;
using System.Linq;
using Features.Buildings.BuildMenu;
using Features.Buildings.Placement;
using Features.Buildings.Structures;
using Features.Common;
using Features.Health;
using Features.Resources;
using Features.Tasks;
using Features.Ui.Actions;
using Features.Ui.UserInput;
using Grimity.Collections;
using Grimity.Data;
using UnityEngine;

namespace Features.Buildings.UI {
public class BuildingManager : Manager<BuildingManager>, IKeyUpReceiver, IInputYielder {
    [SerializeField] private PlacementSettings placementSettings;
    [SerializeField] private GameObject constructionSitePrefab;
    [SerializeField] private LayerMask terrainLayer = 0;
    public IObservable<ActionEntryData[]> BuildingOptions => _buildingOptions;
    public IObservable<Building[]> ExistingBuildings => _existingBuildings;

    private readonly Dictionary<BuildingMenuEntry, int>
        _buildCount = new Dictionary<BuildingMenuEntry, int>();

    private readonly Observable<ActionEntryData[]> _buildingOptions =
        new Observable<ActionEntryData[]>(new ActionEntryData[0]);

    private readonly Observable<Building[]> _existingBuildings =
        new Observable<Building[]>(new Building[] { });

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

    public event YieldControlHandler YieldControl;

    public void OnKeyUp(HashSet<KeyCode> keys, HashSet<KeyCode> pressedKeys, MouseLocation mouseLocation) {
        if (keys.Contains(KeyCode.Escape)) {
            StopBuilding();
        }

        if (keys.Contains(KeyCode.Mouse0) && _hasActivePlaceable) {
            var createdBuilding = PlaceBuilding();
            var canBuildAnother = _buildCount[_selected] > 0;
            var continueBuilding = pressedKeys.Contains(KeyCode.LeftShift) && canBuildAnother;
            if (createdBuilding && !continueBuilding) {
                StopBuilding();
            }
        }
    }

    #endregion

    public void AddBuildingOption(BuildingMenuEntry buildingMenuEntry) {
        var currentCount = _buildCount.GetOrCompute(buildingMenuEntry, entry => 0);
        _buildCount[buildingMenuEntry] = ++currentCount;
        UpdateBuildingOptions();
    }

    private void UpdateSelectedBuilding() {
        if (!_hasActivePlaceable) return;
        _placeable.MayBePlaced.Set(_resourceManager.CanBePayed(_selected.cost));
    }

    private void UpdateBuildingOptions() {
        var actions = _buildCount.Where(pair => pair.Value > 0)
            .Select(pair => {
                var (building, availableCount) = pair;
                return new ActionEntryData {
                    Image = building.image,
                    Active = availableCount > 0 && _resourceManager.CanBePayed(building.cost),
                    OnSelect = () => SelectBuilding(building)
                };
            })
            .ToArray();
        _buildingOptions.Set(actions);
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
        constructionSite.Init(_selected,
            () => {
                _buildCount[_selected]++;
                UpdateBuildingOptions();
            });
        _taskManager.Enqueue(new SimpleTask(constructionSiteGameObject, TaskType.Build));

        _buildCount[_selected]--;
        UpdateBuildingOptions();
    }

    private void StopBuilding() {
        RemoveActivePlaceable();
        YieldControl?.Invoke(this, new YieldControlEventArgs(true));
    }

    private void RemoveActivePlaceable() {
        _hasActivePlaceable = false;
        if (_placeable != null) {
            Destroy(_placeable.gameObject);
        }
    }

    public void RegisterNewBuilding(Building building) {
        var mortal = building.GetComponent<Mortal>();
        if (mortal != null) {
            mortal.onDeath.AddListener(() => {
                var updatedBuildings = _existingBuildings.Value.Where(b => b != building).ToArray();
                _existingBuildings.Set(updatedBuildings);
                _buildCount[building.menuEntry]++;
                UpdateBuildingOptions();
            });
        }

        _existingBuildings.Set(_existingBuildings.Value.Append(building).ToArray());
    }
}
}