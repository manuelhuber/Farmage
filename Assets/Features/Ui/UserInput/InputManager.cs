using System;
using System.Collections.Generic;
using Features.Ui.Selection;
using Grimity.Singleton;
using Grimity.UserInput;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using static Grimity.Cursor.CursorUtil;

namespace Features.Ui.UserInput {
public class InputManager : GrimitySingleton<InputManager> {
    public LayerMask clickableLayers;
    public LayerMask terrainLayers;
    private readonly Stack<IInputReceiver> _memory = new Stack<IInputReceiver>();


    private IInputReceiver _activeReceiver;
    private UnityEngine.Camera _camera;
    private IInputReceiver _defaultReceiver;

    private void Awake() {
        _camera = UnityEngine.Camera.main;
        _activeReceiver = _defaultReceiver = SelectionManager.Instance;
        RequestControl(_defaultReceiver);
    }

    private void Update() {
        SendInputToActiveReceiver();
    }

    private void SendInputToActiveReceiver() {
        if (_activeReceiver == null) return;

        var mouseLocation = GetMouseLocation();

        var pressedKeys = InputUtils.GetCurrentKeys().ToHashSet();
        var downKeys = InputUtils.GetCurrentKeysDown().ToHashSet();
        var upKeys = InputUtils.GetCurrentKeysUp().ToHashSet();

        var mouseOverUi = EventSystem.current.IsPointerOverGameObject();
        if (mouseOverUi) {
            RemoveMouseEvents(pressedKeys);
            RemoveMouseEvents(upKeys);
            RemoveMouseEvents(downKeys);
        }

        _activeReceiver.OnKeyDown(downKeys, mouseLocation);
        _activeReceiver.OnKeyPressed(pressedKeys, mouseLocation);
        _activeReceiver.OnKeyUp(upKeys, mouseLocation);
    }

    private void RemoveMouseEvents(ICollection<KeyCode> keys) {
        keys.Remove(KeyCode.Mouse0);
        keys.Remove(KeyCode.Mouse1);
    }

    public bool RequestControl(IInputReceiver newReceiver) {
        _activeReceiver.YieldControl -= GiveControlToDefaultReceiver;
        _activeReceiver.YieldControl -= PopMemory;
        _activeReceiver = newReceiver;
        _activeReceiver.YieldControl += GiveControlToDefaultReceiver;
        return true;
    }

    public void RequestControlWithMemory(IInputReceiver newReceiver) {
        _memory.Push(_activeReceiver);
        _activeReceiver.YieldControl -= GiveControlToDefaultReceiver;
        _activeReceiver.YieldControl -= PopMemory;

        _activeReceiver = newReceiver;
        _activeReceiver.YieldControl += PopMemory;
    }

    private void PopMemory(object sender, EventArgs e) {
        SetReceiver(_memory.Count > 0 ? _memory.Pop() : _defaultReceiver);
    }

    private void GiveControlToDefaultReceiver(object sender, EventArgs e) {
        SetReceiver(_defaultReceiver);
    }

    private void SetReceiver(IInputReceiver receiver) {
        RequestControl(receiver);
        // Send input to the next receiver in the same frame
        // This way if someone yields control on a mouse0 down the new receiver will immediately get the
        // mouse0 down event too
        SendInputToActiveReceiver();
    }


    private MouseLocation GetMouseLocation() {
        GetCursorLocation(out var terrainHit, _camera, terrainLayers);
        GetCursorLocation(out var clickHit, _camera, clickableLayers);
        var mouseLocation = new MouseLocation
            {Position = terrainHit.point, Collision = clickHit.transform.gameObject};
        return mouseLocation;
    }
}
}