using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Features.Units {
public class Mortal : MonoBehaviour {
    public int hitpoints;
    private Slider _slider;
    public Team team;
    public UnityEvent onDeath;

    private void Start() {
        _slider = GetComponentInChildren<Slider>();
        if (_slider == null) return;
        _slider.maxValue = hitpoints;
        _slider.value = hitpoints;
    }

    public void Die() {
        onDeath.Invoke();
        Destroy(gameObject);
    }

    public void DealDamage(int amount) {
        hitpoints -= amount;
        if (hitpoints > 0) {
            _slider.value = hitpoints;
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