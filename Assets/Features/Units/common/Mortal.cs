﻿using Grimity.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Features.Units.Common {
public class Mortal : MonoBehaviour {
    public UnityEvent onDeath;
    public Team team;

    [field: SerializeField] public int MaxHitpoints { get; private set; }

    public IObservable<int> Hitpoints => _hitpoints;
    private Observable<int> _hitpoints = new Observable<int>(0);

    private void Awake() {
        _hitpoints.Set(MaxHitpoints);
    }

    private void Die() {
        onDeath.Invoke();
        Destroy(gameObject);
    }

    public void TakeDamage(int amount) {
        _hitpoints.Set(_hitpoints.Value - amount);
        if (_hitpoints.Value <= 0) {
            Die();
        }
    }
}

public enum Team {
    Farmers,
    Aliens
}
}