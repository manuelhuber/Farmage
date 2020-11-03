using UnityEngine;
using Werewolf.StatusIndicators.Components;

namespace Features.Abilities.AreaDamage {
[CreateAssetMenu(menuName = "abilities/area damage")]
public class AreaDamageAbility : Ability {
    /// <summary>
    ///     Total arc in degree. 360 means = full circle
    /// </summary>
    public float arc;

    public float radius;
    public int damage;
    public Cone splat;

    public override IAbilityExecutor AddExecutor(GameObject gameObject) {
        var areaDamageExecutor = gameObject.AddComponent<AreaDamageExecutor>();
        areaDamageExecutor.Init(this);
        return areaDamageExecutor;
    }
}
}