using System;
using System.Collections.Generic;
using Features.Building;
using Grimity.Cursor;
using UnityEngine.UI;

namespace UnityEngine {
public class BuildingInput : MonoBehaviour {
    public List<BuildingMenuEntry> entries;
    public LayerMask terrainLayer;

    private Canvas _canvas;
    private GameObject _uiCore;
    private bool _dragObject;
    private Placeable _placeable;
    private Camera _camera;

    private void OnEnable() {
        _camera = GetComponent<Camera>();
        if (_camera == null) _camera = Camera.main;
        InitUi();
    }

    private void Update() {
        if (_dragObject) {
            _placeable.gameObject.transform.position = MouseToTerrain().point - _placeable.lowerCenter;
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            _uiCore.SetActive(true);
            _uiCore.transform.position = Input.mousePosition;
        } else if (Input.GetKeyDown(KeyCode.Escape)) {
            _uiCore.SetActive(false);
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
        CursorUtil.GetCursorLocation(out RaycastHit terrainHit, terrainLayer, _camera, debug: true);
        return terrainHit;
    }
}

[Serializable]
public struct BuildingMenuEntry {
    public GameObject uiPrefab;
    public GameObject buildingPrefab;
}
}