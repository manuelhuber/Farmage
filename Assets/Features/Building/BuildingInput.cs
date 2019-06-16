using System;
using System.Collections.Generic;
using Common.Settings;
using Grimity.Cursor;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Building {
public class BuildingInput : MonoBehaviour {
    public List<BuildingMenuEntry> entries;
    public LayerMask terrainLayer;

    private Canvas _canvas;
    private GameObject _uiCore;
    private bool _dragObject;
    private Placeable _placeable;
    private UnityEngine.Camera _camera;
    private Hotkeys _hotkeys;

    private void Start() {
        _hotkeys = Settings.Instance.Hotkeys;
        _camera = GetComponent<UnityEngine.Camera>();
        if (_camera == null) _camera = UnityEngine.Camera.main;
        InitUi();
    }

    private void Update() {
        if (_dragObject) {
            _placeable.gameObject.transform.position = MouseToTerrain().point - _placeable.lowerCenter;
        }

        if (Input.GetKeyDown(_hotkeys.buildings)) {
            _uiCore.SetActive(true);
            _uiCore.transform.position = Input.mousePosition;
        } else if (Input.GetKeyDown(KeyCode.Escape)) {
            _uiCore.SetActive(false);
            _dragObject = false;
            Destroy(_placeable.gameObject);
        }

        if (Input.GetMouseButtonDown(0) && _dragObject) {
            DetachFromCursor();
        }
    }

    private void InitUi() {
        _canvas = FindObjectOfType<Canvas>();
        _uiCore = Instantiate(new GameObject(), _canvas.transform, false);
        _uiCore.name = "Mouse Centric UI";
        // TODO arrange them nicely
        var offset = 50f;
        foreach (var entry in entries) {
            var button = Instantiate(entry.uiPrefab, _uiCore.transform, false);
            button.transform.localPosition = new Vector3(offset, 0, 0);
            offset += button.GetComponent<RectTransform>().rect.width;
            var buttonClickedEvent = new Button.ButtonClickedEvent();
            buttonClickedEvent.AddListener(() => { AttachToCursor(entry.buildingPrefab); });
            button.GetComponent<Button>().onClick = buttonClickedEvent;
        }

        _uiCore.SetActive(false);
    }

    private void AttachToCursor(GameObject prefab) {
        var building = Instantiate(prefab);
        _placeable = building.AddComponent<Placeable>();
        _placeable.terrainLayer = terrainLayer;
        _uiCore.SetActive(false);
        _dragObject = true;
    }

    private void DetachFromCursor() {
        _dragObject = false;
        _placeable.Place();
    }

    private RaycastHit MouseToTerrain() {
        CursorUtil.GetCursorLocation(out RaycastHit terrainHit, terrainLayer, _camera);
        return terrainHit;
    }
}

[Serializable]
public struct BuildingMenuEntry {
    public GameObject uiPrefab;
    public GameObject buildingPrefab;
}
}