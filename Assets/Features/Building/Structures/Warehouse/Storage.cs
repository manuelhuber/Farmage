using System;
using System.Collections.Generic;
using System.Linq;
using Features.Delivery;
using Features.Resources;
using Features.Save;
using Grimity.Collections;
using Grimity.Data;
using UnityEngine;

namespace Features.Building.Structures.Warehouse {
public class Storage : MonoBehaviour, IDeliveryAcceptor, IDeliveryDispenser,
    ISavableComponent<StorageData> {
    [SerializeField] private Resource[] resources;
    public int capacity;

    private int TotalResourceCount => _storage.Select(pair => pair.Value.Total()).Sum();
    public bool IsFull => TotalResourceCount == capacity;

    private readonly Dictionary<Resource, StoredResource>
        _storage = new Dictionary<Resource, StoredResource>();

    private readonly List<ResourceObject> _waitingForPickup = new List<ResourceObject>();

    public bool AcceptDelivery(GameObject goods) {
        var item = goods.GetComponent<ResourceObject>();
        if (!CanAccept(item)) return false;
        var storedResource = _storage.GetOrDefault(item.resource, new StoredResource());
        storedResource.available += item.count;
        _storage[item.resource] = storedResource;
        Destroy(goods);
        return true;
    }

    public bool DispenseDelivery(GameObject goods) {
        var resourceObject = goods.GetComponent<ResourceObject>();
        if (!_waitingForPickup.Contains(resourceObject)) {
            return false;
        }

        _waitingForPickup.Remove(resourceObject);
        goods.SetActive(true);
        return true;
    }

    public Optional<ResourceObject> ReserveItem(Resource resource, int count) {
        if (!_storage.TryGetValue(resource, out var storedResource) || storedResource.available < count) {
            return Optional<ResourceObject>.NoValue();
        }

        storedResource.available -= count;
        storedResource.reserved += count;
        _storage[resource] = storedResource;

        var resourceObject = resource.CreateResourceObject(count);
        var resourceObjectTransform = resourceObject.transform;
        resourceObjectTransform.parent = transform;
        resourceObjectTransform.localPosition = Vector3.zero;
        _waitingForPickup.Add(resourceObject);
        return resourceObject.AsOptional();
    }

    public bool CanAccept(ResourceObject item) {
        var hasCapacity = item.count + TotalResourceCount <= capacity;
        var storesThisResource = StoresResource(item.resource);
        return hasCapacity && storesThisResource;
    }

    public bool StoresResource(Resource resource) {
        return resources.Contains(resource);
    }

    #region Save

    public string SaveKey => "Storage";

    public StorageData Save() {
        var stored = _storage.ToDictionary(pair => pair.Key.key,
            pair => pair.Value);
        var reserved = _waitingForPickup.Select(o => o.getSaveID()).ToArray();
        return new StorageData {StoredResources = stored, reservedResourceObjectIds = reserved};
    }

    public void Load(StorageData rawData, IReadOnlyDictionary<string, GameObject> objects) {
        foreach (var (resourceKey, storedResource) in rawData.StoredResources) {
            _storage.Clear();
            var res = resources.FirstOrDefault(resource => resource.key == resourceKey);
            if (res == null) {
                Debug.LogWarning(
                    $"Tried to load storage for resource={resourceKey} but this storage doesn't have this resource anymore");
            } else {
                _storage[res] = storedResource;
            }
        }

        foreach (var resourceObjectId in rawData.reservedResourceObjectIds) {
            _waitingForPickup.Clear();
            var resourceObject = objects.getBySaveID(resourceObjectId);
            _waitingForPickup.Add(resourceObject.GetComponent<ResourceObject>());
        }
    }

    #endregion
}

[Serializable]
public struct StoredResource {
    public int available;
    public int reserved;

    public int Total() {
        return available + reserved;
    }
}

[Serializable]
public struct StorageData {
    public string[] reservedResourceObjectIds;
    public Dictionary<string, StoredResource> StoredResources;
}
}