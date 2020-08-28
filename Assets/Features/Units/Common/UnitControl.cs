using System.Collections.Generic;
using Grimity.Data;
using Grimity.Singleton;
using UnityEngine;
using static Grimity.Cursor.CursorUtil;

namespace Features.Units.Common {
public class UnitControl : GrimitySingleton<UnitControl> {
    public LayerMask clickableLayers;
    public Observable<List<Unit>> Selection => _selection;

    private readonly List<Unit> _all = new List<Unit>();
    private Observable<List<Unit>> _selection = new Observable<List<Unit>>(new List<Unit>());


    private UnityEngine.Camera _camera;

    private void Start() {
        _camera = UnityEngine.Camera.main;
    }

    public void Register(Unit unit) {
        _all.Add(unit);
    }

    private void Select(Unit unit) {
        var units = unit == null ? new List<Unit>() : new List<Unit> {unit};
        _selection.Set(units);
    }

    private void Update() {
        var rightClick = Input.GetMouseButtonUp(1);
        var leftClickDown = Input.GetMouseButtonDown(0);
        var leftClickUp = Input.GetMouseButtonUp(0);
        if (!rightClick && !leftClickDown && !leftClickUp) return;

        if (rightClick) {
            var target = MouseToTerrain();
            if (_selection.Value.Count == 0) return;
            foreach (var unit in _selection.Value) unit.SetTarget(target.point);
        } else if (leftClickDown) {
            GetCursorLocation(out var target, _camera);
            var unit = target.transform.gameObject.GetComponent<Unit>();
            Select(unit);
        } else if (leftClickUp) {
            // TODO
        }
    }

    private RaycastHit MouseToTerrain() {
        GetCursorLocation(out var terrainHit, _camera, clickableLayers);
        return terrainHit;
    }
}
}