using System.Collections.Generic;
using Grimity.Singleton;
using UnityEngine;
using static Grimity.Cursor.CursorUtil;

namespace Features.Units {
public class UnitControl : GrimitySingleton<UnitControl> {
    public LayerMask clickableLayers;
    private readonly HashSet<Unit> _selection = new HashSet<Unit>();
    private readonly HashSet<Unit> _all = new HashSet<Unit>();
    private UnityEngine.Camera _camera;


    private void Start() {
        _camera = UnityEngine.Camera.main;
    }

    public void Register(Unit unit) {
        _all.Add(unit);
    }

    private void Select(Unit unit) {
        _selection.Clear();
        _selection.Add(unit);
    }

    private void Update() {
        var rightClick = Input.GetMouseButtonUp(1);
        var leftClickDown = Input.GetMouseButtonDown(0);
        var leftClickUp = Input.GetMouseButtonUp(0);
        if (!rightClick && !leftClickDown && !leftClickUp) return;

        if (rightClick) {
            var target = MouseToTerrain();
            if (_selection.Count == 0) return;
            foreach (var unit in _selection) {
                unit.setTarget(target.point);
            }
        } else if (leftClickDown) {
            GetCursorLocation(out var target, _camera);
            var unit = target.transform.gameObject.GetComponent<Unit>();
            if (unit == null) return;
            _selection.Clear();
            _selection.Add(unit);
        } else if (leftClickUp) {
            // TODO
        }
    }

    private RaycastHit MouseToTerrain() {
        GetCursorLocation(out RaycastHit terrainHit, _camera, clickableLayers);
        return terrainHit;
    }
}
}