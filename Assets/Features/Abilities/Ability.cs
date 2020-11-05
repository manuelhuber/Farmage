using UnityEngine;

namespace Features.Abilities {
public abstract class Ability : ScriptableObject {
    /// <summary>
    ///     In seconds
    /// </summary>
    public float cooldown;

    public string abilityName;
    public string description;
    public Sprite icon;

    public abstract IAbilityExecutor AddExecutor(GameObject gameObject);
}
}