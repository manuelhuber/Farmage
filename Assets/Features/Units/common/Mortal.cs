using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Features.Units {
public class Mortal : MonoBehaviour {
    [SerializeField] private Slider _hitpointBar;
    public UnityEvent onDamage;
    public UnityEvent onDeath;
    public Team team;
    [field: SerializeField] public int MaxHitpoints { get; private set; }
    [field: SerializeField] public int Hitpoints { get; private set; }

    private void Start() {
        Hitpoints = MaxHitpoints;
        _hitpointBar = GetComponentInChildren<Slider>();

        if (_hitpointBar == null) return;
        _hitpointBar.maxValue = Hitpoints;
        _hitpointBar.value = Hitpoints;
    }

    private void Die() {
        onDeath.Invoke();
        Destroy(gameObject);
    }

    public void TakeDamage(int amount) {
        Hitpoints -= amount;
        if (Hitpoints > 0) {
            onDamage.Invoke();
            _hitpointBar.value = Hitpoints;
        } else {
            Die();
        }
    }
}

public enum Team {
    Farmers,
    Aliens
}
}