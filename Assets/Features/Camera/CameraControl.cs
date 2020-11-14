using System.Collections.Generic;
using Features.Ui.Settings;
using Features.Ui.UserInput;
using UnityEngine;

namespace Features.Camera {
public class CameraControl : MonoBehaviour, IOnKeyPressed {
    private Transform _camera;
    private Control _control;
    private Hotkeys _hotkeys;

    private void Start() {
        _hotkeys = Settings.Instance.Hotkeys;
        _control = Settings.Instance.Control;
        _camera = UnityEngine.Camera.main.transform;
        InputManager.Instance.RegisterForPermanentInput(this);
    }

    #region InputReceiver

    public event YieldControlHandler YieldControl;

    public void OnKeyPressed(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
        var movement = new Vector3();
        if (keys.Contains(_hotkeys.moveCameraUp)) movement.z += 1;

        if (keys.Contains(_hotkeys.moveCameraDown)) movement.z -= 1;

        if (keys.Contains(_hotkeys.moveCameraLeft)) movement.x -= 1;

        if (keys.Contains(_hotkeys.moveCameraRight)) movement.x += 1;

        var zoomValue = Input.mouseScrollDelta.y * _control.zoomSpeed;
        _camera.Translate(0, 0, zoomValue);

        movement *= _control.cameraMovementSpeed;
        transform.Translate(movement);
    }

    #endregion
}
}