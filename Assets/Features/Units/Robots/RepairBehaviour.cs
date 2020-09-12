using System.Linq;
using Features.Health;
using Features.Resources;
using Features.Tasks;
using Features.Time;
using Features.Units.Common;
using Grimity.Actions;
using Grimity.Data;
using UnityEngine;

namespace Features.Units.Robots {
public class RepairBehaviour : UnitBehaviourBase<SimpleTask> {
    private MovementAgent _movementAgent;
    private PeriodicalAction _repairAction;
    private ResourceManager _resourceManager;
    private Mortal _target;
    private GameTime _time;

    private void Awake() {
        _time = GameTime.Instance;
        _resourceManager = ResourceManager.Instance;
        _movementAgent = GetComponent<MovementAgent>();
        if (_repairAction == null) _repairAction = gameObject.AddComponent<PeriodicalAction>();

        _repairAction.interval = 1;
        _repairAction.initialDelay = true;
        _repairAction.getTime = () => _time.Time;
        _repairAction.action = () => {
            if (!_resourceManager.Pay(new Cost {cash = 10})) return false;

            _target.TakeDamage(-10);
            return true;
        };
    }


    private void CheckForArrival(Collider[] colliders) {
        if (_target == null) return;
        var arrived = colliders.Any(col => col.gameObject == _target.gameObject);
        _repairAction.IsRunning = arrived;
        _movementAgent.IsStopped = arrived;
    }

    public override void AbandonTask() {
        _repairAction.IsRunning = false;
        base.AbandonTask();
    }

    protected override TaskResponse InitImpl(SimpleTask task, Observable<Collider[]> inRange) {
        _target = task.Payload.GetComponent<Mortal>();
        _target.onDeath.AddListener(Complete);
        _movementAgent.SetDestination(_target.transform.position, true);
        _movementAgent.IsStopped = false;
        inRange.OnChange(CheckForArrival);
        return TaskResponse.Accepted;
    }

    public override void Behave() {
        if (_target.Hitpoints.Value == _target.MaxHitpoints) Complete();
    }

    private void Complete() {
        _repairAction.IsRunning = false;
        CompleteTask();
    }
}
}