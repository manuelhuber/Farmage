using Features.Tasks;
using Grimity.Data;
using UnityEngine;

namespace Features.Delivery {
public class DeliveryTask : BaseTask {
    public DeliveryTask(GameObject goods, Optional<GameObject> origin, Optional<GameObject> destination) {
        Origin = origin;
        Goods = goods;
        Destination = destination;
    }

    public override TaskType Type => TaskType.Deliver;

    public Optional<GameObject> Origin { get; }
    public GameObject Goods { get; }
    public Optional<GameObject> Destination { get; }
}
}