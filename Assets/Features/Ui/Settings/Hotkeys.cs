using UnityEngine;

namespace Features.Ui.Settings {
[CreateAssetMenu(menuName = "Settings/Hotkeys")]
public class Hotkeys : ScriptableObject {
    public KeyCode moveCameraDown = KeyCode.S;
    public KeyCode moveCameraLeft = KeyCode.A;
    public KeyCode moveCameraRight = KeyCode.D;
    public KeyCode moveCameraUp = KeyCode.W;
}
}