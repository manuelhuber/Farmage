using Features.Common;
using Features.Enemies;
using Features.Health;
using UnityEngine;

namespace Features.Buildings.Structures.Scarecrow {
public class AutoTaunt : MonoBehaviour {
    public float range;
    private Mortal _mortal;

    private void Start() {
        _mortal = GetComponent<Mortal>();
        RangeCollider.AddTo(gameObject, range).OnEnter(Taunt);
    }

    private void Taunt(Collider obj) {
        var enemy = obj.GetComponent<EnemyScript>();
        if (enemy == null) return;
        enemy.SetHighPriorityTarget(_mortal);
    }
}
}