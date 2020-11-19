using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.Abilities {
public abstract class Ability : ScriptableObject {
    [BoxGroup("Basics")]
    public string abilityName;

    [BoxGroup("Basics")]
    public string description;

    [BoxGroup("Basics")]
    public Sprite icon;

    [BoxGroup("Basics")]
    public float cooldownInS;

    [BoxGroup("Basics")]
    public string animationTrigger;

    public abstract IAbilityExecutor AddExecutor(GameObject gameObject);
}
}