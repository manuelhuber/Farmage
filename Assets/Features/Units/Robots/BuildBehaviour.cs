using Features.Building.Construction;
using Features.Queue;
using Features.Time;
using Features.Units.Common;
using Grimity.Actions;
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

    private void OnTriggerEnter(Collider other) {
        if (!IsTarget(other)) return;
        _buildAction.IsRunning = true;
        _movementAgent.IsStopped = true;
    }

    private void OnTriggerExit(Collider other) {
        if (!IsTarget(other)) return;
        _buildAction.IsRunning = false;
        _movementAgent.IsStopped = false;
    }

    public override void AbandonTask() {
        _buildAction.IsRunning = false;
        base.AbandonTask();
    }

    protected override bool InitImpl(SimpleTask task) {
        _target = task.Payload.GetComponent<Construction>();
        _movementAgent.SetDestination(_target.transform.position, true);
        _movementAgent.IsStopped = false;
        return true;
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