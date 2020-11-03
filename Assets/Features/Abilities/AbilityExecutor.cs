using UnityEngine;

namespace Features.Abilities {
public abstract class AbilityExecutor : MonoBehaviour {
    public abstract bool CanActivate { get; }
    public abstract void Activate();
    public abstract void Init(Ability ability);
}
}