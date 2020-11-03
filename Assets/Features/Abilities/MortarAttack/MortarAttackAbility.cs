using UnityEngine;
using Werewolf.StatusIndicators.Components;

namespace Features.Abilities.MortarAttack {
[CreateAssetMenu(menuName = "abilities/mortar attack")]
public class MortarAttackAbility : Ability {
    public Point splat;
    public int projectileCount;
    public GameObject projectile;
    public float radius;
    public int damage;

    public override AbilityExecutor AddExecutor(GameObject gameObject) {
        var mortarAttackExecutor = gameObject.AddComponent<MortarAttackExecutor>();
        mortarAttackExecutor.Init(this);
        return mortarAttackExecutor;
    }
}
}