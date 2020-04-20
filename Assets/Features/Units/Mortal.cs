using System;
using Features.Queue;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Features.Units {
[Serializable]
public struct Loot {
    public GameObject prefab;
    public float dropchance;
}

public class Mortal : MonoBehaviour {
    public int hitpoints;
    private Slider _slider;
    public Team team;
    public Loot[] loot;

    private void Start() {
        _slider = GetComponentInChildren<Slider>();
        _slider.maxValue = hitpoints;
        _slider.value = hitpoints;
    }

    public void Die() {
        foreach (var tuple in loot) {
            if (!(Random.value < tuple.dropchance)) continue;
            var newLoot = Instantiate(tuple.prefab, transform.position, transform.rotation);
            Queue.Queue.Enqueue(new Task {type = TaskType.Loot, payload = newLoot});
        }

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