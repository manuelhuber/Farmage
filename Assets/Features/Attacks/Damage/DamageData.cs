using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.Attacks.Damage {
[CreateAssetMenu(menuName = "Attacks/Damage", order = 0)]
public class DamageData : ScriptableObject {
    public int amount;
    public bool dealsAoE;

    [ShowIf("dealsAoE")]
    public float radius;
}
}