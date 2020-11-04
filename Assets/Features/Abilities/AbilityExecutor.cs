using System;
using Features.Time;
using UnityEngine;

namespace Features.Abilities {
public abstract class AbilityExecutor<T> : MonoBehaviour, IAbilityExecutor where T : Ability {
    public T Ability;
    public bool IsOnCooldown => GameTime.getTime() < NextCooldown;
    public float CooldownRemaining => Math.Max(0, NextCooldown - GameTime.getTime());
    protected GameTime GameTime;
    protected float NextCooldown;

    public virtual void Awake() {
        GameTime = GameTime.Instance;
    }

    public virtual bool CanActivate => !IsOnCooldown;

    public abstract void Activate();

    protected void CalculateNextCooldown() {
        NextCooldown = GameTime.getTime() + Ability.cooldown;
    }

    public void Init(T ability) {
        Ability = ability;
        InitImpl(ability);
    }

    protected abstract void InitImpl(T ability);
}

public interface IAbilityExecutor {
    bool CanActivate { get; }
    void Activate();
}
}