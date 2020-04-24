using Common.Settings;
using Features.Building.BuildMenu;
using Features.Building.Placement;
using Grimity.Cursor;
using Grimity.ScriptableObject;
using UnityEngine;
using UnityEngine.UI;
using ResourceManager = Features.Resources.ResourceManager;

namespace Features.Building.UI {
public class BuildingInput : MonoBehaviour {
    public BuildMenu.BuildMenu buildMenu;
    public PlacementSettings placementSettings;
    public LayerMask terrainLayer;
    public GameObject iconPrefab;

    private Canvas _canvas;
    private GameObject _uiCore;
    private bool _dragObject;
    private Placeable _placeable;
    private UnityEngine.Camera _camera;
    private Hotkeys _hotkeys;
    private BuildingMenuEntry _selected;
    private ResourceManager _resourceManager;

    private void Start() {
        _resourceManager = ResourceManager.Instance;
        _hotkeys = Settings.Instance.Hotkeys;
        _camera = GetComponent<UnityEngine.Camera>();
        if (_camera == null) _camera = UnityEngine.Camera.main;
        InitUi();
    }

    private void Update() {
        if (_dragObject) {
            _placeable.transform.position = MouseToTerrain().point;
        }

        if (Input.GetKeyDown(_hotkeys.buildings)) {
            _uiCore.SetActive(true);
            _uiCore.transform.position = Input.mousePosition;
        } else if (Input.GetKeyDown(KeyCode.Escape)) {
            _uiCore.SetActive(false);
            _dragObject = false;
            if (_placeable != null) {
                Destroy(_placeable.gameObject);
                _placeable = null;
            }
        }

        if (Input.GetMouseButtonDown(0) && _dragObject) {
            PlaceBuilding();
        }
    }

    private void InitUi() {
        _canvas = GameObject.FindWithTag("MainUI").GetComponent<Canvas>();
        _uiCore = Instantiate(new GameObject(), _canvas.transform, false);
        _uiCore.name = "Mouse Centric UI";
        // TODO arrange them nicely
        var offset = 50f;
        foreach (var entry in buildMenu.entries) {
            var button = Instantiate(iconPrefab, _uiCore.transform, false);
            button.GetComponent<Image>().sprite = entry.image;
            button.transform.localPosition = new Vector3(offset, 0, 0);
            offset += button.GetComponent<RectTransform>().rect.width;
            void UpdateThisButton(bool playable) => UpdateButton(playable, entry, button);
            var canBePayed = _resourceManager.subscribe(entry.cost, UpdateThisButton);
            UpdateThisButton(canBePayed);
        }

        _uiCore.SetActive(false);
    }

    private void UpdateButton(bool canBePayed, BuildingMenuEntry entry, GameObject button) {
        if (canBePayed) {
            var buttonClickedEvent = new Button.ButtonClickedEvent();
            buttonClickedEvent.AddListener(() => {
                _selected = entry;
                AttachToCursor(entry.previewPrefab);
            });
            button.GetComponent<Button>().onClick = buttonClickedEvent;
            button.GetComponent<Image>().color = Color.white;
        } else {
            button.GetComponent<Image>().color = Color.red;
            button.GetComponent<Button>().onClick = null;
        }
    }

    private void AttachToCursor(GameObject prefab) {
        var building = Instantiate(prefab);
        _placeable = building.AddComponent<Placeable>();
        _placeable.terrainLayer = terrainLayer;
        _placeable.settings = placementSettings;
        _uiCore.SetActive(false);
        _dragObject = true;
    }

    private void PlaceBuilding() {
        if (!_placeable.CanBePlaced) return;
        if (_resourceManager.Pay(_selected.cost)) {
            _placeable.FlattenFloor();
            Instantiate(_selected.buildingPrefab, _placeable.transform.position, _placeable.transform.rotation);
            if (!Input.GetKey(KeyCode.LeftShift)) {
                _dragObject = false;
                Destroy(_placeable.gameObject);
            }
        } else {
            // TODO handle error case
            Debug.Log("Can't pay!");
        }
    }

    private RaycastHit MouseToTerrain() {
        CursorUtil.GetCursorLocation(out var terrainHit, _camera, terrainLayer);
        return terrainHit;
    }
}
}