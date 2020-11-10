using UnityEngine;

namespace Features.Abilities {
public abstract class Ability : ScriptableObject {
    public float cooldownInS;
    public string abilityName;
    public string description;
    public string animationTrigger;
    public Sprite icon;

    public abstract IAbilityExecutor AddExecutor(GameObject gameObject);
}
}