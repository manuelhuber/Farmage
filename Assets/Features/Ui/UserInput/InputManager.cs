using System.Collections.Generic;
using Features.Common;
using Features.Ui.Selection;
using Grimity.UserInput;
using UnityEngine;
using UnityEngine.EventSystems;
using static Grimity.Cursor.CursorUtil;

namespace Features.Ui.UserInput {
public class InputManager : Manager<InputManager> {
    public LayerMask clickableLayers;
    public LayerMask terrainLayers;
    private readonly Stack<IInputReceiver> _memory = new Stack<IInputReceiver>();
    private readonly HashSet<IInputReceiver> _permanentReceiver = new HashSet<IInputReceiver>();


    private IInputReceiver _activeReceiver;
    private UnityEngine.Camera _camera;
    private IInputReceiver _defaultReceiver;
    private HashSet<KeyCode> _downKeys;
    private MouseLocation _mouseLocation;
    private HashSet<KeyCode> _pressedKeys;
    private HashSet<KeyCode> _upKeys;

    private void Awake() {
        _camera = UnityEngine.Camera.main;
        _activeReceiver = _defaultReceiver = SelectionManager.Instance;
        RequestControl(_defaultReceiver);
    }

    private void Update() {
        UpdateCurrentInput();
        if (_activeReceiver != null) {
            SendCurrentInputToReceiver(_activeReceiver);
        }

        foreach (var inputReceiver in _permanentReceiver) {
            SendCurrentInputToReceiver(inputReceiver);
        }
    }

    private void UpdateCurrentInput() {
        _mouseLocation = GetMouseLocation();
        _pressedKeys = InputUtils.GetCurrentKeys();
        _downKeys = InputUtils.GetCurrentKeysDown();
        _upKeys = InputUtils.GetCurrentKeysUp();
        var mouseOverUi = EventSystem.current.IsPointerOverGameObject();
        if (!mouseOverUi) return;
        RemoveMouseEvents(_pressedKeys);
        RemoveMouseEvents(_upKeys);
        RemoveMouseEvents(_downKeys);
    }

    private static void RemoveMouseEvents(ICollection<KeyCode> keys) {
        keys.Remove(KeyCode.Mouse0);
        keys.Remove(KeyCode.Mouse1);
    }

    private void SendCurrentInputToReceiver(IInputReceiver receiver) {
        if (_downKeys.Count > 0) {
            (receiver as IKeyDownReceiver)?.OnKeyDown(_downKeys, _pressedKeys, _mouseLocation);
        }

        if (_pressedKeys.Count > 0 || Input.mouseScrollDelta.y != 0) {
            (receiver as IKeyPressedReceiver)?.OnKeyPressed(_pressedKeys, _mouseLocation);
        }

        if (_upKeys.Count > 0) {
            (receiver as IKeyUpReceiver)?.OnKeyUp(_upKeys, _pressedKeys, _mouseLocation);
        }
    }

    public void RegisterForPermanentInput(IInputReceiver receiver) {
        _permanentReceiver.Add(receiver);
    }

    public void RequestControl(IInputReceiver newReceiver) {
        GiveControl(newReceiver, GiveControlToDefaultReceiver);
    }

    public void RequestControlWithMemory(IInputReceiver newReceiver) {
        _memory.Push(_activeReceiver);
        GiveControl(newReceiver, GiveControlToPreviousReceiver);
    }

    private void GiveControl(IInputReceiver newReceiver, YieldControlHandler newReceiverOnYield) {
        if (_activeReceiver is IInputYielder previousYielder) {
            previousYielder.YieldControl -= GiveControlToDefaultReceiver;
            previousYielder.YieldControl -= GiveControlToPreviousReceiver;
        }

        _activeReceiver = newReceiver;
        if (newReceiver is IInputYielder newYielder) {
            newYielder.YieldControl += newReceiverOnYield;
        }

        (newReceiver as IControlReceiver)?.OnReceiveControl();
    }

    private void GiveControlToPreviousReceiver(IInputReceiver inputReceiver, YieldControlEventArgs args) {
        SetReceiverAfterYield(_memory.Count > 0 ? _memory.Pop() : _defaultReceiver, args);
    }

    private void GiveControlToDefaultReceiver(IInputReceiver inputReceiver, YieldControlEventArgs args) {
        SetReceiverAfterYield(_defaultReceiver, args);
    }

    private void SetReceiverAfterYield(IInputReceiver receiver, YieldControlEventArgs yieldArgs) {
        RequestControl(receiver);
        if (!yieldArgs.ConsumedKeyEvent) {
            // Send input to the next receiver in the same frame
            // This way if someone yields control on a mouse0 down the new receiver will immediately get the
            // mouse0 down event too
            SendCurrentInputToReceiver(receiver);
        }
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