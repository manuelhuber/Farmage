using UnityEngine;

namespace Features.Abilities.AutoAttack {
[CreateAssetMenu(menuName = "abilities/auto attack")]
public class AutoAttackAbility : Ability {
    public int damage;
    public float range;
    public bool shootDuringMove;

    public override IAbilityExecutor AddExecutor(GameObject gameObject) {
        var autoAttackExecutor = gameObject.AddComponent<AutoAttackExecutor>();
        autoAttackExecutor.Init(this);
        return autoAttackExecutor;
    }
}
}