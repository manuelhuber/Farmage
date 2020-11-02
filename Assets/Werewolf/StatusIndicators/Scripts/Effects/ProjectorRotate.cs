using UnityEngine;

namespace Werewolf.StatusIndicators.Effects {
public class ProjectorRotate : MonoBehaviour {
    public float RotationSpeed;

    private void LateUpdate() {
        transform.eulerAngles = new Vector3(90, Mathf.Repeat(Time.time * RotationSpeed, 360), 0);
    }
}
}