﻿using Features.Queue;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Features.Units.Robots {
public class LootGatherer : MonoBehaviour, WokerbotBehaviour {
    private GameObject _loot;
    private bool _isCarryingLoot;
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private JobQueue jobQueue;

    private void Start() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public int ActivationPriority() {
        return jobQueue.Count();
    }

    public void Behave() {
        if (_loot == null) {
            var dequeue =
                jobQueue.Dequeue(task => Vector3.Distance(task.payload.transform.position, transform.position));
            if (dequeue == null) return;
            _loot = dequeue.Value.payload;
            _navMeshAgent.SetDestination(_loot.transform.position);
        } else {
            if (!((transform.position - _navMeshAgent.destination).magnitude < 5)) return;
            if (!_isCarryingLoot) {
                PickupLoot();
            } else {
                DropLoot();
            }
        }
    }

    private void DropLoot() {
        _loot.transform.parent = null;
        _loot = null;
        _isCarryingLoot = false;
    }

    private void PickupLoot() {
        _loot.transform.parent = transform;
        var hq = GameObject.FindGameObjectWithTag("HQ");
        _navMeshAgent.SetDestination(hq.transform.position);
        _isCarryingLoot = true;
    }

    public void Abandon() {
    }
}
}