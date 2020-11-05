using UnityEngine;

namespace Features.Abilities.Shielding {
[CreateAssetMenu(menuName = "abilities/shielding ", order = 0)]
public class ShieldingAbility : Ability {
    public int shieldAmount;

    /// <summary>
    ///     In seconds
    /// </summary>
    public float duration;

    public override IAbilityExecutor AddExecutor(GameObject gameObject) {
        var shieldingExecutor = gameObject.AddComponent<ShieldingExecutor>();
        shieldingExecutor.Init(this);
        return shieldingExecutor;
    }
}
}