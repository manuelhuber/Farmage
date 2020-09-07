using System.Collections.Generic;
using Grimity.Data;
using Grimity.Singleton;
using UnityEngine;
using UnityEngine.EventSystems;
using static Grimity.Cursor.CursorUtil;

namespace Features.Ui.Selection {
public class SelectionManager : GrimitySingleton<SelectionManager> {
    public LayerMask clickableLayers;

    public Observable<List<Selectable>> Selection { get; } =
        new Observable<List<Selectable>>(new List<Selectable>());

    private readonly List<Selectable> _all = new List<Selectable>();
    private UnityEngine.Camera _camera;

    private void Start() {
        _camera = UnityEngine.Camera.main;
    }

    private void Update() {
        var clickedOnUi = EventSystem.current.IsPointerOverGameObject();
        if (clickedOnUi) return;
        var rightClick = Input.GetMouseButtonUp(1);
        var leftClickDown = Input.GetMouseButtonDown(0);
        var leftClickUp = Input.GetMouseButtonUp(0);
        if (!rightClick && !leftClickDown && !leftClickUp) return;

        if (rightClick) {
            var target = MouseToTerrain();
            if (Selection.Value.Count == 0) return;
            Debug.Log($"Right clicked with selection on {target.ToString()}");
        } else if (leftClickDown) {
            GetCursorLocation(out var target, _camera);
            var unit = target.transform.gameObject.GetComponent<Selectable>();
            Select(unit);
        } else if (leftClickUp) {
            // TODO
        }
    }

    public void Register(Selectable selectable) {
        _all.Add(selectable);
    }

    private void Select(Selectable selectable) {
        var units = selectable == null ? new List<Selectable>() : new List<Selectable> {selectable};
        Selection.Set(units);
    }

    private RaycastHit MouseToTerrain() {
        GetCursorLocation(out var terrainHit, _camera, clickableLayers);
        return terrainHit;
    }
}
}