using Features.Ui.Settings;
using UnityEngine;

namespace Features.Camera {
public class CameraControl : MonoBehaviour {
    private Transform _camera;
    private Control _control;
    private Hotkeys _hotkeys;

    private void Start() {
        _hotkeys = Settings.Instance.Hotkeys;
        _control = Settings.Instance.Control;
        _camera = UnityEngine.Camera.main.transform;
    }

    // Update is called once per frame
    private void Update() {
        var movement = new Vector3();
        var rotation = new Vector3();
        if (Input.GetKey(_hotkeys.moveCameraUp)) movement.z += 1;

        if (Input.GetKey(_hotkeys.moveCameraDown)) movement.z -= 1;

        if (Input.GetKey(_hotkeys.moveCameraLeft)) movement.x -= 1;

        if (Input.GetKey(_hotkeys.moveCameraRight)) movement.x += 1;

        if (Input.GetKey(_hotkeys.turnCameraLeft)) rotation.y += 1;

        if (Input.GetKey(_hotkeys.turnCameraRight)) rotation.y -= 1;

        var zoomValue = Input.mouseScrollDelta.y * _control.zoomSpeed;
        _camera.Translate(0, 0, zoomValue);

        rotation *= _control.cameraRotateSpeed;
        transform.Rotate(rotation);

        movement *= _control.cameraMovementSpeed;
        transform.Translate(movement);
    }
}
}