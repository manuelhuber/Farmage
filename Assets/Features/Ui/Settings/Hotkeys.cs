using UnityEngine;

namespace Features.Ui.Settings {
[CreateAssetMenu(menuName = "Settings/Hotkeys")]
public class Hotkeys : ScriptableObject {
    public KeyCode buildings = KeyCode.B;
    public KeyCode moveCameraDown = KeyCode.S;
    public KeyCode moveCameraLeft = KeyCode.A;
    public KeyCode moveCameraRight = KeyCode.D;
    public KeyCode moveCameraUp = KeyCode.W;
    public KeyCode turnCameraLeft = KeyCode.Q;
    public KeyCode turnCameraRight = KeyCode.E;
}
}