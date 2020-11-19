using Features.Attacks.Damage;
using Features.Attacks.Trajectory;
using Sirenix.OdinInspector;
using UnityEngine;
using Vendor.Werewolf.StatusIndicators.Scripts.Components;

namespace Features.Abilities.MortarAttack {
[CreateAssetMenu(menuName = "abilities/mortar attack")]
public class MortarAttackAbility : Ability {
    public Point splat;
    public int projectileCount;
    public float range;

    [InlineEditor]
    public DamageData damage;

    [AssetsOnly]
    public GameObject projectile;

    [ShowIf("projectile")]
    public Trajectory trajectory;


    public override IAbilityExecutor AddExecutor(GameObject gameObject) {
        var mortarAttackExecutor = gameObject.AddComponent<MortarAttackExecutor>();
        mortarAttackExecutor.Init(this);
        return mortarAttackExecutor;
    }
}
}