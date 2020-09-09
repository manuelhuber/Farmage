using System.Linq;
using Features.Building.Construction;
using Features.Tasks;
using Features.Time;
using Features.Units.Common;
using Grimity.Actions;
using Grimity.Data;
using UnityEngine;

namespace Features.Units.Robots {
public class BuildBehaviour : UnitBehaviourBase<SimpleTask> {
    private PeriodicalAction _buildAction;
    private MovementAgent _movementAgent;
    private Construction _target;
    private GameTime _time;

    private void Awake() {
        _time = GameTime.Instance;
        _movementAgent = GetComponent<MovementAgent>();
        if (_buildAction == null) _buildAction = gameObject.AddComponent<PeriodicalAction>();

        _buildAction.interval = 1;
        _buildAction.initialDelay = true;
        _buildAction.getTime = () => _time.Time;
        _buildAction.action = () => {
            if (_target.Build(10)) {
                Complete();
            }

            return true;
        };
    }

    public override void AbandonTask() {
        _buildAction.IsRunning = false;
        base.AbandonTask();
    }

    protected override bool InitImpl(SimpleTask task, Observable<Collider[]> inRange) {
        _target = task.Payload.GetComponent<Construction>();
        _movementAgent.SetDestination(_target.transform.position, true);
        _movementAgent.IsStopped = false;
        inRange.OnChange(CheckColliders);
        return true;
    }

    private void CheckColliders(Collider[] colliders) {
        var targetIsInRange = colliders.Any(IsTarget);
        _buildAction.IsRunning = targetIsInRange;
        _movementAgent.IsStopped = targetIsInRange;
    }

    private void Complete() {
        _buildAction.IsRunning = false;
        CompleteTask();
    }

    private bool IsTarget(Component other) {
        return _target != null && _target.gameObject == other.gameObject;
    }
}
}