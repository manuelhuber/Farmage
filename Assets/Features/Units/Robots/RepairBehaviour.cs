using Features.Health;
using Features.Queue;
using Features.Resources;
using Features.Time;
using Features.Units.Common;
using Grimity.Actions;
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

    private void OnTriggerEnter(Collider other) {
        if (_target == null || _target.gameObject != other.gameObject) return;
        _repairAction.IsRunning = true;
        _movementAgent.IsStopped = true;
    }

    private void OnTriggerExit(Collider other) {
        if (_target == null || _target.gameObject != other.gameObject) return;
        _repairAction.IsRunning = false;
        _movementAgent.IsStopped = false;
    }

    public override void AbandonTask() {
        _repairAction.IsRunning = false;
        base.AbandonTask();
    }

    protected override bool InitImpl(SimpleTask task) {
        _target = task.Payload.GetComponent<Mortal>();
        _target.onDeath.AddListener(Complete);
        _movementAgent.SetDestination(_target.transform.position, true);
        _movementAgent.IsStopped = false;
        return true;
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