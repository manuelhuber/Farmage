using System;
using System.Collections.Generic;
using System.Linq;
using Features.Building.Structures.Warehouse;
using Features.Items;
using Features.Queue;
using Features.Save;
using Features.Units.Common;
using Grimity.ScriptableObject;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;

namespace Features.Units.Robots {
public class LootGatherer : UnitBehaviourBase, ISavableComponent {
    private GameObject _loot;
    private MovementAgent _movementAgent;
    private Storage _targetStorage;
    public RuntimeGameObjectSet buildings;
    private bool _isCarryingLoot;

    private void Awake() {
        _movementAgent = GetComponent<MovementAgent>();
    }

    public override bool Init(Task task) {
        var target = FindStorage(task.payload);
        if (target == null) return false;
        StartGathering(task.payload, target);
        return true;
    }

    private Storage FindStorage(GameObject loot) {
        var storable = loot.GetComponent<Storable>();
        return buildings.Items.Select(o => o.GetComponent<Storage>())
            .Where(storage => storage != null)
            .FirstOrDefault(storage => storable.IsType(storage.type));
    }

    private void StartGathering(GameObject loot, Storage storage) {
        _loot = loot;
        _isCarryingLoot = false;
        _targetStorage = storage;
        _movementAgent.SetDestination(_loot.transform.position);
        _movementAgent.IsStopped = false;
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

    private void DeliverLoot() {
        if (!_targetStorage.Deliver(_loot.GetComponent<Storable>())) {
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
}

[Serializable]
internal struct LootGathererData {
    public string loot;
    public string targetStorage;
    public bool isCarryingLoot;
}
}