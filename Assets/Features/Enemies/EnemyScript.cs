using System;
using Features.Health;
using Grimity.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Features.Enemies {
public class EnemyScript : MonoBehaviour {
    public int damage = 5;
    private GameObject[] _targets;
    private GameObject _target;
    private NavMeshAgent _navMeshAgent;
    private CapsuleCollider _collider;

    private void Start() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<CapsuleCollider>();
    }

    private void Update() {
        if (_targets.Length == 0) return;
        if (_target == null) {
            _target = _targets.GetRandomElement();
        }

        _navMeshAgent.SetDestination(_target.transform.position);
    }

    public void setTargets(GameObject[] targets) {
        _targets = targets;
    }

    private void OnTriggerEnter(Collider other) {
        var destructible = other.gameObject.GetComponent<Destructible>();
        if (destructible == null || destructible.team == Team.Aliens) return;
        destructible.DealDamage(damage);
        Destroy(gameObject);
    }
}
}