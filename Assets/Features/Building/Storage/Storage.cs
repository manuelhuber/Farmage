using Features.Delivery;
using Features.Resources;
using Grimity.Data;
using UnityEngine;

namespace Features.Building.Storage {
public abstract class Storage : MonoBehaviour, IDeliveryAcceptor, IDeliveryDispenser {
    public abstract int TotalResourceCount { get; }
    public abstract bool IsFull { get; }
    public abstract bool AcceptDelivery(GameObject goods);
    public abstract bool DispenseDelivery(GameObject goods);
    public abstract bool CanAccept(ResourceObject item);
    public abstract bool StoresResource(Resource resource);
    public abstract Optional<ResourceObject> ReserveItem(Resource resource, int count);
}
}