using UnityEngine;

namespace Vendor.Werewolf.StatusIndicators.Scripts.Effects {
public class ProjectorFixedRotation : MonoBehaviour {
    public float Angle;

    private void LateUpdate() {
        transform.eulerAngles = new Vector3(90, Angle, 0);
    }
}
}