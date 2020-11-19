using UnityEngine;

namespace Features.Projectile {
[CreateAssetMenu(menuName = "Trajectory")]
public class Trajectory : ScriptableObject {
    public AnimationCurve height;
    public AnimationCurve speed;
    public float baseSpeed;
    public GameObject impactFx;
}
}