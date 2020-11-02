using UnityEngine;

namespace Features.Units.Walkers.Abilities {
[CreateAssetMenu(menuName = "abilities/area damage")]
public class AreaDamageAbility : Ability {
    /// <summary>
    ///     Total arc in degree. 360 means everything
    /// </summary>
    public float arc;

    public float radius;
    public int damage;

    /// <summary>
    ///     Time between activation and execution
    /// </summary>
    public float chargeTime;

    public GameObject splat;

    public override IAbilityExecutor AddExecutor(GameObject gameObject) {
        var areaDamageExecutor = gameObject.AddComponent<AreaDamageExecutor>();
        areaDamageExecutor.Init(this);
        return areaDamageExecutor;
    }
}
}