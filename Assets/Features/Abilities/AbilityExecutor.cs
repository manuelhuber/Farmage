using System;
using Features.Time;
using UnityEngine;

namespace Features.Abilities {
public abstract class AbilityExecutor<T> : MonoBehaviour, IAbilityExecutor where T : Ability {
    public T ability;
    public bool IsOnCooldown => GameTime.getTime() < NextCooldown;
    protected GameTime GameTime;
    protected float NextCooldown;

    public virtual void Awake() {
        GameTime = GameTime.Instance;
    }

    public float CooldownRemaining => Math.Max(0, NextCooldown - GameTime.getTime());

    public virtual bool CanActivate => !IsOnCooldown;

    public abstract void Activate();

    protected void CalculateNextCooldown() {
        NextCooldown = GameTime.getTime() + ability.cooldownInS;
    }

    public void Init(T ab) {
        ability = ab;
    }

}

public interface IAbilityExecutor {
    bool CanActivate { get; }
    float CooldownRemaining { get; }
    void Activate();
}
}