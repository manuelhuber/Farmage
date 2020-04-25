using System;
using System.Linq;
using Features.Building.Structures.Warehouse;
using Features.Items;
using Features.Queue;
using Grimity.ScriptableObject;
using UnityEngine;
using UnityEngine.AI;

namespace Features.Units.Robots {
public class LootGatherer : UnitBehaviourBase {
    private GameObject _loot;
    private Storage _targetStorage;
    private NavMeshAgent _navMeshAgent;
    public RuntimeGameObjectSet buildings;

    private void Start() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public override bool Init(Task task) {
        Start();
        var item = task.payload.GetComponent<Storable>();
        var target = buildings.Items.Select(o => o.GetComponent<Storage>())
            .Where(storage => storage != null).FirstOrDefault(storage => item.isType(storage._type));
        if (target == null) {
            return false;
        }

        _loot = task.payload;
        _targetStorage = target;
        _navMeshAgent.SetDestination(_loot.transform.position);
        _navMeshAgent.isStopped = false;
        return true;
    }

    private void OnTriggerEnter(Collider other) {
        if (_loot == null) return;
        if (other.gameObject == _loot.gameObject) {
            PickupLoot();
        } else if (other.gameObject == _targetStorage.gameObject) {
            DeliverLoot();
        }
    }

    private void DeliverLoot() {
        _loot.transform.parent = null;
        _targetStorage.Deliver(_loot.GetComponent<Storable>());
        _navMeshAgent.isStopped = true;
        _loot = null;
        CompleteTask();
    }

    private void PickupLoot() {
        _loot.transform.parent = transform;
        _navMeshAgent.SetDestination(_targetStorage.transform.position);
    }

    public override void AbandonTask() {
        DropLoot();
        _navMeshAgent.isStopped = true;
        base.AbandonTask();
    }

    private void DropLoot() {
        if (_loot == null) return;
        _loot.transform.parent = null;
        _loot = null;
    }
}
}