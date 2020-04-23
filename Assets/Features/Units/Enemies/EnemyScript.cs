﻿using System;
using System.Linq;
using Features.Units;
using Grimity.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Features.Enemies {
public class EnemyScript : MonoBehaviour {
    public int damage = 5;
    private GameObject[] _targets;
    private GameObject _target;
    private NavMeshAgent _navMeshAgent;

    private void Start() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        if (_targets.Length == 0) return;
        if (_target == null) {
            if (_targets.All(t => t == null)) return;
            _target = _targets.GetRandomElement();
        }

        _navMeshAgent.SetDestination(_target.transform.position);
    }

    public void setTargets(GameObject[] targets) {
        _targets = targets;
    }

    private void OnTriggerEnter(Collider other) {
        var destructible = other.gameObject.GetComponent<Mortal>();
        if (destructible == null || destructible.team == Team.Aliens) return;
        destructible.TakeDamage(damage);
        Destroy(gameObject);
    }
}
}