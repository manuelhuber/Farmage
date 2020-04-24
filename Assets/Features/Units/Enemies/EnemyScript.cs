﻿using System;
using System.Linq;
using Features.Units;
using Grimity.Actions;
using Grimity.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Features.Enemies {
public class EnemyScript : MonoBehaviour {
    public int damage = 5;
    public float attackSpeed;
    private GameObject[] _targets;
    private NavMeshAgent _navMeshAgent;
    private Mortal _victim;
    private IntervaledAction _attack;


    private void Start() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _attack = gameObject.AddComponent<IntervaledAction>();
        _attack.interval = attackSpeed;
        _attack.action = () => { _victim.TakeDamage(damage); };
        FindNewTarget();
    }

    private void Update() {
        if (_attack.IsRunning) {
            return;
        }
    }

    public void setTargets(GameObject[] targets) {
        _targets = targets;
    }

    private void OnTriggerExit(Collider other) {
        if (_victim == null) return;
        if (other.gameObject != _victim.gameObject) return;
        Debug.Log("Too far from victim: " + Vector3.Distance(_victim.transform.position, transform.position));
        StopAttack();
        _navMeshAgent.SetDestination(_victim.transform.position);
    }

    private void OnTriggerEnter(Collider other) {
        var destructible = other.gameObject.GetComponent<Mortal>();
        if (destructible == null || destructible.team == Team.Aliens) return;
        Debug.Log("Picked target");
        transform.LookAt(destructible.transform);
        _navMeshAgent.isStopped = true;
        _victim = destructible;
        _attack.IsRunning = true;
        _victim.onDeath.AddListener(() => {
            Debug.Log("target dead");
            StopAttack();
        });
    }

    private void StopAttack() {
        _attack.IsRunning = false;
        _navMeshAgent.isStopped = false;

        FindNewTarget();
    }

    private void FindNewTarget() {
        if (_targets.Length == 0) return;
        if (_targets.All(t => t == null)) return;
        var target = _targets.GetRandomElement();

        _navMeshAgent.SetDestination(target.transform.position);
    }
}
}