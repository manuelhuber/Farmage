﻿using System;
using System.Collections.Generic;
using Features.Ui.UserInput;
using Grimity.Data;
using Grimity.Singleton;
using UnityEngine;

namespace Features.Ui.Selection {
public class SelectionManager : GrimitySingleton<SelectionManager>, IInputReceiver {
    public Observable<List<Selectable>> Selection { get; } =
        new Observable<List<Selectable>>(new List<Selectable>());

    private InputManager _inputManager;

    private void Start() {
        _inputManager = InputManager.Instance;
    }

    #region InputReceiver

    public event EventHandler YieldControl;

    public void OnKeyDown(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
        if (keys.Contains(KeyCode.Mouse0)) {
            // TODO Start box dragging
        }
    }

    public void OnKeyPressed(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
    }

    public void OnKeyUp(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
        if (keys.Contains(KeyCode.Mouse0)) {
            var selection = FinishSelection(mouseLocation);
            if (keys.Contains(KeyCode.LeftShift)) {
            } else {
                Select(selection);
            }
        }

        if (keys.Contains(KeyCode.Mouse1)) {
            foreach (var selectable in Selection.Value) {
                // pass right click to selection?
            }
        }

        if (keys.Contains(KeyCode.Escape)) {
            Unselect();
        }
    }

    #endregion

    private List<Selectable> FinishSelection(MouseLocation mouseLocation) {
        // TODO get selection from box dragging
        var selectable = mouseLocation.Collision.GetComponent<Selectable>();
        return selectable == null ? new List<Selectable>() : new List<Selectable> {selectable};
    }

    private void Unselect() {
        Select(new List<Selectable>());
    }

    private void Select(List<Selectable> selectable) {
        Selection.Set(selectable);
        if (selectable.Count == 1) {
            var selectedUnit = selectable[0].GetComponent<IInputReceiver>();
            if (selectedUnit != null) {
                _inputManager.RequestControl(selectedUnit);
            }
        }
    }
}
}