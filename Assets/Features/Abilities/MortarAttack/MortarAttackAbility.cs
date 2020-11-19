using Features.Projectile;
using UnityEngine;
using Vendor.Werewolf.StatusIndicators.Scripts.Components;

namespace Features.Abilities.MortarAttack {
[CreateAssetMenu(menuName = "abilities/mortar attack")]
public class MortarAttackAbility : Ability {
    public Point splat;
    public int projectileCount;
    public GameObject projectile;
    public Trajectory trajectory;
    public float radius;
    public int damage;
    public float range;

    public override IAbilityExecutor AddExecutor(GameObject gameObject) {
        var mortarAttackExecutor = gameObject.AddComponent<MortarAttackExecutor>();
        mortarAttackExecutor.Init(this);
        return mortarAttackExecutor;
    }
}
}