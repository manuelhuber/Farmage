using System;
using System.Collections;
using System.Collections.Generic;
using Features.Queue;
using Features.Units.Robots;
using UnityEngine;
using UnityEngine.AI;
using Queue = Features.Queue.Queue;

public class LootGatherer : MonoBehaviour, WokerbotBehaviour {
    private GameObject loot;
    private bool isCarryingLoot;
    private NavMeshAgent _navMeshAgent;

    private void Start() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public int ActivationPriority() {
        return Queue.Count(TaskType.Loot);
    }

    public void Behave() {
        if (loot == null) {
            var dequeue = Queue.Dequeue(TaskType.Loot);
            if (dequeue == null) return;
            loot = dequeue.Value.payload;
            _navMeshAgent.SetDestination(loot.transform.position);
        } else {
            if (!((transform.position - _navMeshAgent.destination).magnitude < 5)) return;
            if (!isCarryingLoot) {
                PickupLoot();
            } else {
                DropLoot();
            }
        }
    }

    private void DropLoot() {
        loot.transform.parent = null;
        loot = null;
        isCarryingLoot = false;
    }

    private void PickupLoot() {
        loot.transform.parent = transform;
        var hq = GameObject.FindGameObjectWithTag("HQ");
        _navMeshAgent.SetDestination(hq.transform.position);
        isCarryingLoot = true;
    }

    public void Abandon() {
    }
}