using System.Collections.Generic;
using System.Linq;
using Features.Resources;
using Grimity.Collections;
using Grimity.Data;
using UnityEngine;

namespace Features.Building.Structures.Warehouse {
public class BasicStorage : Storage.Storage {
    [SerializeField] private Resource[] resources;
    public int capacity;

    public override int TotalResourceCount => _storage.Select(pair => pair.Value.Total()).Sum();
    public override bool IsFull => TotalResourceCount == capacity;

    private readonly Dictionary<Resource, StoredResource>
        _storage = new Dictionary<Resource, StoredResource>();

    private readonly List<ResourceObject> _waitingForPickup = new List<ResourceObject>();

    public override bool AcceptDelivery(GameObject goods) {
        var item = goods.GetComponent<ResourceObject>();
        if (!CanAccept(item)) return false;
        var storedResource = _storage.GetOrDefault(item.resource, new StoredResource());
        storedResource.Available += item.count;
        _storage[item.resource] = storedResource;
        Destroy(goods);
        return true;
    }

    public override bool DispenseDelivery(GameObject goods) {
        var resourceObject = goods.GetComponent<ResourceObject>();
        if (!_waitingForPickup.Contains(resourceObject)) {
            return false;
        }

        _waitingForPickup.Remove(resourceObject);
        goods.SetActive(true);
        return true;
    }

    public override Optional<ResourceObject> ReserveItem(Resource resource, int count) {
        if (!_storage.TryGetValue(resource, out var storedResource) || storedResource.Available < count) {
            return Optional<ResourceObject>.NoValue();
        }

        storedResource.Available -= count;
        storedResource.Reserved += count;
        _storage[resource] = storedResource;

        var resourceObject = resource.CreateResourceObject(count);
        var resourceObjectTransform = resourceObject.transform;
        resourceObjectTransform.parent = transform;
        resourceObjectTransform.localPosition = Vector3.zero;
        _waitingForPickup.Add(resourceObject);
        return resourceObject.AsOptional();
    }

    public override bool CanAccept(ResourceObject item) {
        var hasCapacity = item.count + TotalResourceCount <= capacity;
        var storesThisResource = StoresResource(item.resource);
        return hasCapacity && storesThisResource;
    }

    public override bool StoresResource(Resource resource) {
        return resources.Contains(resource);
    }
}

public struct StoredResource {
    public int Available;
    public int Reserved;

    public int Total() {
        return Available + Reserved;
    }
}
}