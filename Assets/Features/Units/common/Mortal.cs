using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Features.Units {
public class Mortal : MonoBehaviour {
    [SerializeField] private int hitpoints;
    [SerializeField] private Slider _hitpointBar;
    public Team team;
    [SerializeField] private UnityEvent onDeath;

    private void Start() {
        _hitpointBar = GetComponentInChildren<Slider>();
        if (_hitpointBar == null) return;
        _hitpointBar.maxValue = hitpoints;
        _hitpointBar.value = hitpoints;
    }

    private void Die() {
        onDeath.Invoke();
        Destroy(gameObject);
    }

    public void TakeDamage(int amount) {
        hitpoints -= amount;
        if (hitpoints > 0) {
            _hitpointBar.value = hitpoints;
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