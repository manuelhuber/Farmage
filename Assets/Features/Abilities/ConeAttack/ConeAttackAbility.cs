using Features.Attacks.Damage;
using Sirenix.OdinInspector;
using UnityEngine;
using Vendor.Werewolf.StatusIndicators.Scripts.Components;

namespace Features.Abilities.ConeAttack {
[CreateAssetMenu(menuName = "abilities/cone attack")]
public class ConeAttackAbility : Ability {
    /// <summary>
    ///     Total arc in degree. 360 means = full circle
    /// </summary>
    public float arc;

    [InlineEditor]
    public DamageData damage;

    public Cone splat;

    public override IAbilityExecutor AddExecutor(GameObject gameObject) {
        var areaDamageExecutor = gameObject.AddComponent<AreaDamageExecutor>();
        areaDamageExecutor.Init(this);
        return areaDamageExecutor;
    }
}
}