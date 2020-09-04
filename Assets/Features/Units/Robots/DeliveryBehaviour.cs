using System;
using System.Collections.Generic;
using Features.Building.Structures.Warehouse;
using Features.Delivery;
using Features.Save;
using Features.Units.Common;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;

namespace Features.Units.Robots {
public class DeliveryBehaviour : UnitBehaviourBase<DeliveryTask>, ISavableComponent {
    private bool _isCarryingLoot;
    private GameObject _loot;
    private MovementAgent _movementAgent;
    private Storage _targetStorage;

    private void Awake() {
        _movementAgent = GetComponent<MovementAgent>();
    }

    private void OnTriggerEnter(Collider other) {
        if (_loot == null) return;
        var o = other.gameObject;
        var isLoot = o == _loot.gameObject;
        var isStorage = o == _targetStorage.gameObject;
        if (isLoot) {
            PickupLoot();
        } else if (_isCarryingLoot && isStorage) {
            DeliverLoot();
        }
    }

    public string SaveKey => "LootGatherer";

    public string Save() {
        return new LootGathererData {
            loot = _loot.getSaveID(), isCarryingLoot = _isCarryingLoot,
            targetStorage = _targetStorage.getSaveID()
        }.ToJson();
    }

    public void Load(string rawData, IReadOnlyDictionary<string, GameObject> objects) {
        var data = rawData.FromJson<LootGathererData>();
        var loot = objects.getBySaveID(data.loot);
        var targetStorage = objects.getBySaveID(data.targetStorage);
        if (loot == null) return;
        StartGathering(loot, targetStorage.GetComponent<Storage>());
        if (!data.isCarryingLoot) return;
        PickupLoot();
    }

    protected override bool InitImpl(DeliveryTask task) {
        var target = task.Target;
        if (target == null) return false;
        StartGathering(task.Goods, target.GetComponent<Storage>());
        return true;
    }

    private void StartGathering(GameObject loot, Storage storage) {
        _loot = loot;
        _isCarryingLoot = false;
        _targetStorage = storage;
        _movementAgent.SetDestination(_loot.transform.position);
        _movementAgent.IsStopped = false;
    }

    private void DeliverLoot() {
        if (!_targetStorage.AcceptDelivery(_loot)) {
            Debug.LogWarning("Couldn't deliver item - code a solution for this problem!");
            return;
        }

        _isCarryingLoot = false;
        _movementAgent.IsStopped = true;
        _loot = null;
        CompleteTask();
    }

    private void PickupLoot() {
        _loot.transform.parent = transform;
        _isCarryingLoot = true;
        _movementAgent.SetDestination(_targetStorage.transform.position, true);
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

[Serializable]
internal struct LootGathererData {
    public string loot;
    public string targetStorage;
    public bool isCarryingLoot;
}
}