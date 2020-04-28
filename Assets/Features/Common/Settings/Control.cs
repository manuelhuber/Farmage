using UnityEngine;

namespace Features.Common.Settings {
[CreateAssetMenu(menuName = "Settings/Control")]
public class Control : ScriptableObject {
    public float cameraMovementSpeed = 1f;
    public float cameraRotateSpeed = 1f;
    public float zoomSpeed = 1f;
}
}