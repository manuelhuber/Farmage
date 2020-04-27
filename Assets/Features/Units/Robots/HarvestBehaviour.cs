using Features.Building.Structures.WheatField;
using Features.Queue;
using Features.Resources;
using Features.Units.Common;
using UnityEngine;

namespace Features.Units.Robots {
public class HarvestBehaviour : UnitBehaviourBase {
    private MovementAgent _movementAgent;
    private ResourceManager _resourceManager;
    private GameObject target;

    private void Awake() {
        _resourceManager = ResourceManager.Instance;
        _movementAgent = GetComponent<MovementAgent>();
    }

    public override bool Init(Task task) {
        target = task.payload;
        _movementAgent.SetDestination(target.transform.position);
        _movementAgent.isStopped = false;
        return true;
    }

    public override void Behave() {
        if (!(_movementAgent.hasArrived)) return;
        var harvest = target.GetComponent<WheatField>().harvest();
        _resourceManager.Add(harvest);
        CompleteTask();
    }
}
}