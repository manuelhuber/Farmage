using Features.Attacks.Damage;
using Features.Attacks.Trajectory;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.Abilities.AutoAttack {
[CreateAssetMenu(menuName = "abilities/auto attack")]
public class AutoAttackAbility : Ability {
    [BoxGroup("Attack")]
    public float range;

    [BoxGroup("Attack")]
    public bool shootDuringMove;

    [BoxGroup("Attack")]
    [InlineEditor]
    public DamageData damage;

    [BoxGroup("Attack")]
    public GameObject projectile;

    [BoxGroup("Attack")]
    [ShowIf("projectile")]
    [InlineEditor]
    public Trajectory trajectory;

    public override IAbilityExecutor AddExecutor(GameObject gameObject) {
        var autoAttackExecutor = gameObject.AddComponent<AutoAttackExecutor>();
        autoAttackExecutor.Init(this);
        return autoAttackExecutor;
    }
}
}