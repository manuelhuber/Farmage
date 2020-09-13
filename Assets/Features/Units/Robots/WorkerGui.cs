using Features.Delivery;
using Features.Tasks;
using Features.Ui.Selection;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Units.Robots {
public class WorkerGui : MonoBehaviour, ISingleSelectionDetailGui {
    public Text text;
    private Worker _worker;

    private void Update() {
        var task = _worker.CurrentTask;
        switch (task) {
            case null:
                text.text = "Waiting for work";
                break;
            case DeliveryTask deliveryTask:
                text.text = $"Delivering {deliveryTask.Goods.name} to {deliveryTask.Destination.Value.name}";
                break;
            case SimpleTask simpleTask:
                switch (task.Type) {
                    case TaskType.Harvest:
                        text.text = $"Harvesting from {simpleTask.Payload.name}";
                        break;
                    case TaskType.Repair:
                        text.text = $"Repairing {simpleTask.Payload.name}";
                        break;
                    case TaskType.Build:
                        text.text = $"Building {simpleTask.Payload.name}";
                        break;
                }

                break;
        }
    }


    public void Init(GameObject selectedUnit) {
        _worker = selectedUnit.GetComponent<Worker>();
    }
}
}