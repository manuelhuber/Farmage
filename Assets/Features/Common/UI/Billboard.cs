using UnityEngine;

namespace Features.Common.UI {
public class Billboard : MonoBehaviour {
    private UnityEngine.Camera _mCamera;

    private void Start() {
        _mCamera = UnityEngine.Camera.main;
    }

    //Orient the camera after all movement is completed this frame to avoid jittering
    private void LateUpdate() {
        var rotation = _mCamera.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward,
            rotation * Vector3.up);
    }
}
}