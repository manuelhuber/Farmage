using Features.Building.Structures.WheatField;
using Features.Queue;
using Features.Resources;
using Features.Units.Common;
using UnityEngine;

namespace Features.Units.Robots {
public class HarvestBehaviour : UnitBehaviourBase {
    private MovementAgent _movementAgent;
    private ResourceManager _resourceManager;
    private GameObject _target;

    private void Awake() {
        _resourceManager = ResourceManager.Instance;
        _movementAgent = GetComponent<MovementAgent>();
    }

    public override bool Init(Task task) {
        _target = task.payload;
        _movementAgent.SetDestination(_target.transform.position);
        _movementAgent.IsStopped = false;
        return true;
    }

    public override void Behave() {
        if (!(_movementAgent.HasArrived)) return;
        var harvest = _target.GetComponent<WheatField>().Harvest();
        _resourceManager.Add(harvest);
        CompleteTask();
    }
}
}