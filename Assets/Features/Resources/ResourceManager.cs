﻿using System;
using System.Collections.Generic;
using System.Linq;
using Features.Buildings.Storage;
using Features.Buildings.Structures;
using Features.Buildings.UI;
using Features.Common;
using Features.Delivery;
using Features.Tasks;
using Grimity.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Resources {
public class ResourceManager : Manager<ResourceManager> {
    public Cost startingCash;
    public Grimity.Data.IObservable<Cost> Have => _have;
    private readonly Observable<Cost> _have = new Observable<Cost>(new Cost());
    private List<Storage> _storages = new List<Storage>();
    private TaskManager _taskManager;

    private Text _text;

    private List<ResourceObject> _waitingForStorage = new List<ResourceObject>();


    private void Start() {
        _taskManager = TaskManager.Instance;
        Add(startingCash);
        BuildingManager.Instance.ExistingBuildings.OnChange(OnBuildingChange);
    }

    public Cost Add(Cost change) {
        _have.Set(_have.Value + change);
        return _have.Value;
    }

    public bool Pay(Cost change) {
        if (!CanBePayed(change)) return false;

        _have.Set(_have.Value - change);
        return true;
    }

    public bool CanBePayed(Cost cost) {
        return cost <= _have.Value;
    }

    public Optional<Tuple<ResourceObject, Storage>> ReserveItem(Resource resource) {
        var itemAndStorage = _storages
            .Select(s => Tuple.Create(s.ReserveItem(resource, 1), s))
            .FirstOrDefault(tuple => tuple.Item1.HasValue);
        if (itemAndStorage == null) {
            return Optional<Tuple<ResourceObject, Storage>>.NoValue();
        }

        var storage = itemAndStorage.Item2;
        var item = itemAndStorage.Item1;
        return Tuple.Create(item.Value, storage).AsOptional();
    }

    public void RegisterNewResource(ResourceObject resourceObject) {
        var resourceIsBeingStored = EnqueueDeliveryToStorage(resourceObject);
        if (!resourceIsBeingStored) {
            _waitingForStorage.Add(resourceObject);
        }
    }

    private void OnBuildingChange(Building[] buildings) {
        _storages = buildings.Select(o => o.GetComponent<Storage>())
            .Where(storage => storage != null)
            .ToList();
        _waitingForStorage = _waitingForStorage.Where(res => !EnqueueDeliveryToStorage(res)).ToList();
    }

    private Optional<Storage> GetBestStorage(ResourceObject newLoot) {
        return _storages
            .FirstOrDefault(storage => storage.CanAccept(newLoot))
            .AsOptional();
    }

    private bool EnqueueDeliveryToStorage(ResourceObject resourceObject) {
        var storage = GetBestStorage(resourceObject);
        if (!storage.HasValue) {
            return false;
        }

        var deliveryTask =
            new DeliveryTask(resourceObject.gameObject,
                Optional<GameObject>.NoValue(),
                storage.Value.gameObject.AsOptional());
        _taskManager.Enqueue(deliveryTask);
        return true;
    }
}
}