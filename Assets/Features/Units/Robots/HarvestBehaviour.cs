using Features.Buildings.Structures.WheatField;
using Features.Tasks;
using Features.Units.Common;
using Grimity.Data;
using UnityEngine;

namespace Features.Units.Robots {
public class HarvestBehaviour : UnitBehaviourBase<SimpleTask> {
    private MovementAgent _movementAgent;
    private GameObject _target;

    private void Awake() {
        _movementAgent = GetComponent<MovementAgent>();
    }

    protected override TaskResponse InitImpl(SimpleTask task, Observable<Collider[]> inRange) {
        _target = task.Payload;
        _movementAgent.SetDestination(_target.transform.position, true);
        _movementAgent.IsStopped = false;
        return TaskResponse.Accepted;
    }

    public override void Behave() {
        if (!_movementAgent.HasArrived) return;
        _target.GetComponent<WheatField>().Harvest();
        CompleteTask();
    }
}
}