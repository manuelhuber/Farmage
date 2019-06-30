using System;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Health {
public class Destructible : MonoBehaviour {
    public int hitpoints;
    private Slider _slider;
    public Team team;

    private void Start() {
        _slider = GetComponentInChildren<Slider>();
        _slider.maxValue = hitpoints;
        _slider.value = hitpoints;
    }

    public void DealDamage(int amount) {
        hitpoints -= amount;
        if (hitpoints > 0) {
            _slider.value = hitpoints;
        } else {
            Die();
        }
    }

    private void Die() {
        Destroy(gameObject);
    }
}

public enum Team {
    Farmers,
    Aliens
}
}