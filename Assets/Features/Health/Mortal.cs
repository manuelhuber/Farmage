using System;
using System.Collections.Generic;
using Features.Save;
using Grimity.Data;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;
using UnityEngine.Events;

namespace Features.Health {
public class Mortal : MonoBehaviour, ISavableComponent {
    public UnityEvent onDeath = new UnityEvent();
    public Team team;

    [field: SerializeField] public int MaxHitpoints { get; set; }

    public Grimity.Data.IObservable<int> Hitpoints => _hitpoints;
    private readonly Observable<int> _hitpoints = new Observable<int>(0);

    private void Awake() {
        _hitpoints.Set(MaxHitpoints);
    }

    private void Die() {
        onDeath.Invoke();
        Destroy(gameObject);
    }

    public void TakeDamage(int amount) {
        _hitpoints.Set(Mathf.Min(_hitpoints.Value - amount, MaxHitpoints));
        if (_hitpoints.Value <= 0) {
            Die();
        }
    }

    #region Save

    public string SaveKey => "Mortal";

    public string Save() {
        return new MortalData {hp = _hitpoints.Value}.ToJson();
    }

    public void Load(string rawData, IReadOnlyDictionary<string, GameObject> objects) {
        _hitpoints.Set(rawData.FromJson<MortalData>().hp);
    }

    [Serializable]
    private struct MortalData {
        public int hp;
    }

    #endregion
}

public enum Team {
    Farmers,
    Aliens
}
}