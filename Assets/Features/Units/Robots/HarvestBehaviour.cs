using Features.Building.Structures.WheatField;
using Features.Tasks;
using Features.Units.Common;
using UnityEngine;

namespace Features.Units.Robots {
public class HarvestBehaviour : UnitBehaviourBase<SimpleTask> {
    private MovementAgent _movementAgent;
    private GameObject _target;

    private void Awake() {
        _movementAgent = GetComponent<MovementAgent>();
    }

    protected override bool InitImpl(SimpleTask task) {
        _target = task.Payload;
        _movementAgent.SetDestination(_target.transform.position);
        _movementAgent.IsStopped = false;
        return true;
    }

    public override void Behave() {
        if (!_movementAgent.HasArrived) return;
        _target.GetComponent<WheatField>().Harvest();
        CompleteTask();
    }
}
}