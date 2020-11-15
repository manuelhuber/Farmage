using UnityEngine;

namespace Vendor.Werewolf.StatusIndicators.Scripts.Effects {
public class FixedRotation : MonoBehaviour {
    public Vector3 Rotation;

    private void LateUpdate() {
        transform.eulerAngles = Rotation;
    }
}
}