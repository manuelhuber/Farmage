using System.Collections.Generic;
using Features.Save;
using Features.Tasks;
using Grimity.Data;
using UnityEngine;

namespace Features.Delivery {
public class DeliveryTask : BaseTask, ISavableComponent<DeliveryTaskData> {
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

    public string SaveKey => SaveKeyStatic;

    public DeliveryTaskData Save() {
        return new DeliveryTaskData {
            Origin = Origin.HasValue ? Origin.Value.getSaveID() : "",
            Goods = Goods.getSaveID(),
            Destination = Destination.HasValue ? Destination.Value.getSaveID() : ""
        };
    }

    public void Load(DeliveryTaskData data, IReadOnlyDictionary<string, GameObject> objects) {
        Goods = objects.getBySaveID(data.Goods);
        Origin = objects.getBySaveID(data.Origin).AsOptional();
        Destination = objects.getBySaveID(data.Destination).AsOptional();
    }
}

public struct DeliveryTaskData {
    public string Origin;
    public string Goods;
    public string Destination;
}
}