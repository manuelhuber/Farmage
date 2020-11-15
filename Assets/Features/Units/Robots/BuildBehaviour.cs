using System.Linq;
using Features.Buildings.Construction;
using Features.Tasks;
using Features.Time;
using Features.Units.Common;
using Grimity.Actions;
using Grimity.Data;
using UnityEngine;

namespace Features.Units.Robots {
public class BuildBehaviour : UnitBehaviourBase<SimpleTask> {
    [SerializeField] private int constructionProgress = 10;

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
            var buildCompleted = _target == null || _target.Build(constructionProgress);
            if (buildCompleted) {
                Complete();
            }

            return true;
        };
    }

    public override void AbandonTask() {
        _buildAction.IsRunning = false;
        base.AbandonTask();
    }

    protected override TaskResponse InitImpl(SimpleTask task, Observable<Collider[]> inRange) {
        var construction = task.Payload;
        if (construction == null) return TaskResponse.Completed;
        _target = construction.GetComponent<Construction>();
        _movementAgent.SetDestination(_target.transform.position, true);
        _movementAgent.IsStopped = false;
        inRange.OnChange(CheckColliders);
        return TaskResponse.Accepted;
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
        return _target != null && other != null && _target.gameObject == other.gameObject;
    }
}
}