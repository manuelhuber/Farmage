using System.Linq;
using Features.Building.BuildMenu;
using Features.Building.Placement;
using Features.Resources;
using Features.Tasks;
using Features.Ui.Actions;
using Grimity.Data;
using Grimity.Singleton;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Building.UI {
public class BuildingManager : GrimitySingleton<BuildingManager> {
    public static readonly int GridSize = 4;

    [SerializeField] private PlacementSettings placementSettings;
    [SerializeField] private GameObject constructionSitePrefab;
    [SerializeField] private LayerMask terrainLayer = 0;
    [SerializeField] private BuildMenu.BuildMenu buildMenu;

    private readonly Observable<ActionEntry[]> _buildingOptions =
        new Observable<ActionEntry[]>(new ActionEntry[0]);

    private bool _hasActivePlaceable;
    private Placeable _placeable;
    private ResourceManager _resourceManager;
    private BuildingMenuEntry _selected;
    private TaskManager _taskManager;
    public IObservable<ActionEntry[]> BuildingOptions => _buildingOptions;

    private void Awake() {
        _taskManager = TaskManager.Instance;
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

        if (!_hasActivePlaceable || !Input.GetMouseButtonDown(0)) return;
        var clickedOnUi = EventSystem.current.IsPointerOverGameObject();
        if (clickedOnUi) return;
        PlaceBuilding();
    }

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
        if (_hasActivePlaceable) {
            DetachFromCursor();
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

    private void PlaceBuilding() {
        if (!_placeable.CanBePlaced) {
            Debug.Log("Can't build here!");
            return;
        }

        if (_resourceManager.Pay(_selected.cost)) {
            CreateConstructionSite();
            if (!Input.GetKey(KeyCode.LeftShift)) {
                DetachFromCursor();
            }
        } else {
            // TODO handle error case
            Debug.Log("Can't pay!");
        }
    }

    private void CreateConstructionSite() {
        var location = _placeable.transform;
        var constructionSiteGameObject =
            Instantiate(constructionSitePrefab, location.position, location.rotation);
        var constructionSite = constructionSiteGameObject.GetComponent<Construction.Construction>();
        constructionSite.Init(_selected);

        _taskManager.Enqueue(new SimpleTask {type = TaskType.Build, Payload = constructionSiteGameObject});
    }

    private void DetachFromCursor() {
        _hasActivePlaceable = false;
        if (_placeable != null) {
            Destroy(_placeable.gameObject);
        }
    }
}
}