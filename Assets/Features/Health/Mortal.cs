using System;
using System.Collections.Generic;
using Features.Save;
using Grimity.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Features.Health {
public class Mortal : MonoBehaviour, ISavableComponent<MortalData>, ITeam {
    public UnityEvent onDeath = new UnityEvent();
    [SerializeField] private Team team;

    [field: SerializeField] public int MaxHitpoints { get; set; }

    public Grimity.Data.IObservable<int> Hitpoints => _hitpoints;
    public Grimity.Data.IObservable<int> Shield => _shield;
    public Grimity.Data.IObservable<int> MaxShield => _maxShield;
    private readonly Observable<int> _hitpoints = new Observable<int>(0);
    private readonly Observable<int> _maxShield = new Observable<int>(0);
    private readonly Observable<int> _shield = new Observable<int>(0);

    public readonly List<Func<Damage, Damage>> OnDamageInterceptor = new List<Func<Damage, Damage>>();

    private void Awake() {
        _hitpoints.Set(MaxHitpoints);
    }

    public Team Team => team;

    public void ChangeMaxShield(int change) {
        _maxShield.Set(Math.Max(_maxShield.Value + change, 0));
        var newShield = change > 0
            ? _shield.Value + change
            : Math.Min(_shield.Value, _maxShield.Value);
        _shield.Set(newShield);
    }

    public void TakeDamage(Damage damage) {
        foreach (var interceptor in OnDamageInterceptor) {
            damage = interceptor.Invoke(damage);
        }

        var absorbed = 0;
        if (!damage.IsHeal && _shield.Value > 0) {
            absorbed = Math.Min(damage.Amount, _shield.Value);
            _shield.Set(_shield.Value - absorbed);
        }

        var remainingDamage = damage.Amount - absorbed;
        if (remainingDamage == 0) return;
        _hitpoints.Set(Mathf.Min(_hitpoints.Value - remainingDamage, MaxHitpoints));
        if (_hitpoints.Value <= 0) {
            Die();
        }
    }

    private void Die() {
        onDeath.Invoke();
        Destroy(gameObject);
    }

    #region Save

    public string SaveKey => "Mortal";

    public MortalData Save() {
        return new MortalData {
            hp = _hitpoints.Value,
            currentShield = _shield.Value,
            maxShield = _maxShield.Value
        };
    }

    public void Load(MortalData data, IReadOnlyDictionary<string, GameObject> objects) {
        _hitpoints.Set(data.hp);
        _maxShield.Set(data.maxShield);
        _shield.Set(data.currentShield);
    }

    #endregion
}

[Serializable]
public struct MortalData {
    public int hp;
    public int maxShield;
    public int currentShield;
}
}