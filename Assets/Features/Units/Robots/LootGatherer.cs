using System.Linq;
using Features.Building.Structures.Warehouse;
using Features.Items;
using Features.Queue;
using Features.Units.Common;
using Grimity.ScriptableObject;
using UnityEngine;

namespace Features.Units.Robots {
public class LootGatherer : UnitBehaviourBase {
    private GameObject _loot;
    private MovementAgent _movementAgent;
    private Storage _targetStorage;
    public RuntimeGameObjectSet buildings;

    private void Awake() {
        _movementAgent = GetComponent<MovementAgent>();
    }

    public override bool Init(Task task) {
        var item = task.payload.GetComponent<Storable>();
        var target = buildings.Items.Select(o => o.GetComponent<Storage>())
            .Where(storage => storage != null)
            .FirstOrDefault(storage => item.IsType(storage.type));
        if (target == null) return false;

        _loot = task.payload;
        _targetStorage = target;
        _movementAgent.SetDestination(_loot.transform.position);
        _movementAgent.IsStopped = false;
        return true;
    }

    private void OnTriggerEnter(Collider other) {
        if (_loot == null) return;
        if (other.gameObject == _loot.gameObject)
            PickupLoot();
        else if (other.gameObject == _targetStorage.gameObject) DeliverLoot();
    }

    private void DeliverLoot() {
        _loot.transform.parent = null;
        _targetStorage.Deliver(_loot.GetComponent<Storable>());
        _movementAgent.IsStopped = true;
        _loot = null;
        CompleteTask();
    }

    private void PickupLoot() {
        _loot.transform.parent = transform;
        _movementAgent.SetDestination(_targetStorage.transform.position);
    }

    public override void AbandonTask() {
        DropLoot();
        _movementAgent.IsStopped = true;
        base.AbandonTask();
    }

    private void DropLoot() {
        if (_loot == null) return;
        _loot.transform.parent = null;
        _loot = null;
    }
}
}