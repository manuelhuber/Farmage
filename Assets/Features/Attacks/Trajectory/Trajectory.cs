using UnityEngine;

namespace Features.Attacks.Trajectory {
[CreateAssetMenu(menuName = "Attacks/Trajectory")]
public class Trajectory : ScriptableObject {
    public AnimationCurve height;
    public AnimationCurve speed;
    public float baseSpeed;
    public GameObject impactFx;
}
}