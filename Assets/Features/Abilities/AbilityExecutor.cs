using System;
using Features.Animations;
using Features.Time;
using JetBrains.Annotations;
using UnityEngine;

namespace Features.Abilities {
public abstract class AbilityExecutor<T> : MonoBehaviour, IAbilityExecutor where T : Ability {
    public T ability;
    public bool IsOnCooldown => GameTime.getTime() < NextCooldown;
    protected AnimationHandler AnimationHandler;
    protected GameTime GameTime;
    protected float NextCooldown;

    public virtual void Awake() {
        GameTime = GameTime.Instance;
        AnimationHandler = gameObject.GetComponentInChildren<AnimationHandler>();
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

    [UsedImplicitly]
    public virtual void AnimationCallback(string animationTrigger) {
        if (animationTrigger == ability.animationTrigger) {
            AnimationCallbackImpl();
        }
    }

    protected virtual void AnimationCallbackImpl() {
    }
}

public interface IAbilityExecutor {
    bool CanActivate { get; }
    float CooldownRemaining { get; }
    void Activate();
}
}