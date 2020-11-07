using UnityEngine;

namespace Werewolf.StatusIndicators.Effects {
public class FixedRotation : MonoBehaviour {
    public Vector3 Rotation;

    private void LateUpdate() {
        transform.eulerAngles = Rotation;
    }
}
}