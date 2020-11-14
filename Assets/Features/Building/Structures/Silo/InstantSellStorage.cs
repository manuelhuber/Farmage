using System.Linq;
using Features.Resources;
using Grimity.Data;
using UnityEngine;

namespace Features.Building.Structures.Silo {
/// <summary>
///     Will sell any item stored here instantly.
///     Can't dispense items obviously
/// </summary>
public class InstantSellStorage : Storage.Storage {
    [SerializeField] private Resource[] resources;

    public override int TotalResourceCount => 0;
    public override bool IsFull => false;
    private ResourceManager _resourceManager;

    private void Awake() {
        _resourceManager = ResourceManager.Instance;
    }

    public override bool CanAccept(ResourceObject item) {
        return StoresResource(item.resource);
    }

    public override bool StoresResource(Resource resource) {
        return resources.Contains(resource);
    }

    public override Optional<ResourceObject> ReserveItem(Resource resource, int count) {
        return Optional<ResourceObject>.NoValue();
    }

    public override bool AcceptDelivery(GameObject goods) {
        var resourceObject = goods.GetComponent<ResourceObject>();
        _resourceManager.Add(resourceObject.resource.worth);
        Destroy(goods);
        return true;
    }

    public override bool DispenseDelivery(GameObject goods) {
        return false;
    }
}
}