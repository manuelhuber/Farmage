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

    public static string SaveKeyStatic => "DeliveryTask";
    public override TaskType Type => TaskType.Deliver;

    public Optional<GameObject> Origin { get; private set; }
    public GameObject Goods { get; private set; }
    public Optional<GameObject> Destination { get; private set; }
}
}