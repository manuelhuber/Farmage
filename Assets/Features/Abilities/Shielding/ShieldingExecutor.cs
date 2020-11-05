using Features.Health;
using Grimity.Actions;

namespace Features.Abilities.Shielding {
public class ShieldingExecutor : AbilityExecutor<ShieldingAbility> {
    public override void Activate() {
        var mortal = GetComponent<Mortal>();
        mortal.ChangeMaxShield(ability.shieldAmount);
        this.Do(() => mortal.ChangeMaxShield(-ability.shieldAmount)).After(ability.duration);
        CalculateNextCooldown();
    }

    protected override void InitImpl() {
    }
}
}