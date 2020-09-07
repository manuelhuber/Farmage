using System;
using System.Collections.Generic;
using Features.Delivery;
using Features.Save;
using Features.Units.Common;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;

namespace Features.Units.Robots {
public class DeliveryBehaviour : UnitBehaviourBase<DeliveryTask>, ISavableComponent {
    private GameObject _destination;
    private GameObject _goods;
    private bool _isCarryingGoods;
    private MovementAgent _movementAgent;
    private GameObject _originStorage;

    private void Awake() {
        _movementAgent = GetComponent<MovementAgent>();
    }

    private void OnTriggerEnter(Collider other) {
        if (_goods == null) return;
        var o = other.gameObject;
        var arrivedAtGoods = o == _goods.gameObject || o == _originStorage;
        var arrivedAtStorage = o == _destination.gameObject;
        if (arrivedAtGoods) {
            PickupLoot();
        } else if (_isCarryingGoods && arrivedAtStorage) {
            DeliverLoot();
        }
    }

    protected override bool InitImpl(DeliveryTask task) {
        var target = task.Target;
        if (target == null) return false;
        StartGathering(task.Goods, target, task.From);
        return true;
    }

    private void StartGathering(GameObject goods, GameObject destination, GameObject origin) {
        _goods = goods;
        _isCarryingGoods = false;
        _destination = destination;
        _originStorage = origin;
        _movementAgent.SetDestination(_goods.transform.position, true);
        _movementAgent.IsStopped = false;
    }

    private void DeliverLoot() {
        if (!_destination.GetComponent<IDeliveryAcceptor>().AcceptDelivery(_goods)) {
            Debug.LogWarning("Couldn't deliver item - code a solution for this problem!");
            return;
        }

        _isCarryingGoods = false;
        _movementAgent.IsStopped = true;
        _goods = null;
        CompleteTask();
    }

    private void PickupLoot() {
        if (_originStorage != null) {
            var dispenser = _originStorage.GetComponent<IDeliveryDispenser>();
            dispenser.DispenseDelivery(_goods);
        }

        _goods.transform.parent = transform;
        _isCarryingGoods = true;
        _movementAgent.SetDestination(_destination.transform.position, true);
    }

    public override void AbandonTask() {
        DropLoot();
        _movementAgent.IsStopped = true;
        base.AbandonTask();
    }

    private void DropLoot() {
        if (_goods == null) return;
        _goods.transform.parent = null;
        _goods = null;
    }

    #region Save

    public string SaveKey => "LootGatherer";

    public string Save() {
        return new LootGathererData {
            loot = _goods.getSaveID(), isCarryingLoot = _isCarryingGoods,
            targetStorage = _destination.getSaveID(),
            originStorage = _originStorage.getSaveID()
        }.ToJson();
    }

    public void Load(string rawData, IReadOnlyDictionary<string, GameObject> objects) {
        var data = rawData.FromJson<LootGathererData>();
        var loot = objects.getBySaveID(data.loot);
        var targetStorage = objects.getBySaveID(data.targetStorage);
        var originStorage = objects.getBySaveID(data.originStorage);
        if (loot == null) return;
        StartGathering(loot, targetStorage, originStorage);
        if (!data.isCarryingLoot) return;
        PickupLoot();
    }

    [Serializable]
    private struct LootGathererData {
        public string loot;
        public string targetStorage;
        public string originStorage;
        public bool isCarryingLoot;
    }

    #endregion
}
}