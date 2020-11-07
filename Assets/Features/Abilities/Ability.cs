using UnityEngine;

namespace Features.Abilities {
public abstract class Ability : ScriptableObject {
    public float cooldownInS;
    public string abilityName;
    public string description;
    public Sprite icon;

    public abstract IAbilityExecutor AddExecutor(GameObject gameObject);
}
}