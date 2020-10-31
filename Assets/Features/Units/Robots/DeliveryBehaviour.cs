using System.Linq;
using Features.Delivery;
using Features.Units.Common;
using Grimity.Data;
using UnityEngine;

namespace Features.Units.Robots {
public class DeliveryBehaviour : UnitBehaviourBase<DeliveryTask> {
    private GameObject _destination;
    private GameObject _goods;
    private bool _isCarryingGoods;
    private MovementAgent _movementAgent;
    private Optional<GameObject> _originStorage;

    private void Awake() {
        _movementAgent = GetComponent<MovementAgent>();
    }

    protected override TaskResponse InitImpl(DeliveryTask task, Observable<Collider[]> inRange) {
        if (!task.Destination.HasValue) {
            Debug.LogWarning($"Received delivery task for goods={task.Goods.name} without destination");
            return TaskResponse.Declined;
        }

        StartGathering(task.Goods, task.Destination.Value, task.Origin);
        inRange.OnChange(CheckForArrival);
        return _goods == null ? TaskResponse.Completed : TaskResponse.Accepted;
    }

    private void StartGathering(GameObject goods,
                                GameObject destination,
                                Optional<GameObject> origin) {
        _goods = goods;
        _isCarryingGoods = false;
        _destination = destination;
        _originStorage = origin;
        var firstLocation = origin.HasValue ? origin.Value : goods;
        _movementAgent.SetDestination(firstLocation.transform.position, true);
        _movementAgent.IsStopped = false;
    }

    private void CheckForArrival(Collider[] colliders) {
        if (_goods == null) return;
        var objectsInRange = colliders.Select(col => col.gameObject).ToArray();
        var arrivedAtPickup = objectsInRange.Any(IsPickupLocation);
        var arrivedAtDropOff = objectsInRange.Any(obj => obj == _destination.gameObject);
        if (arrivedAtPickup) {
            PickupLoot();
        }

        if (_isCarryingGoods && arrivedAtDropOff) {
            DeliverLoot();
        }
    }

    private bool IsPickupLocation(GameObject gameObj) {
        return _originStorage.HasValue ? _originStorage.Value == gameObj : _goods.gameObject == gameObj;
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
        if (_originStorage.HasValue) {
            var dispenser = _originStorage.Value.GetComponent<IDeliveryDispenser>();
            dispenser.DispenseDelivery(_goods);
        }

        _goods.transform.parent = transform;
        _goods.transform.localPosition = Vector3.zero;
        _isCarryingGoods = true;
        _movementAgent.SetDestination(_destination.transform.position, true);
    }

    public override void AbandonTask() {
        // TODO - if we abandon a task with origin store after we've picked up the item already we'll
        // be in a weird state that will most likely cause bugs
        DropLoot();
        _movementAgent.IsStopped = true;
        base.AbandonTask();
    }

    private void DropLoot() {
        _isCarryingGoods = false;
        if (_goods == null) return;
        _goods.transform.parent = null;
        _goods = null;
    }
}
}