using Features.Queue;
using Features.Resources;
using Features.Units.Common;
using Grimity.Actions;
using UnityEngine;

namespace Features.Units.Robots {
public class RepairBehaviour : UnitBehaviourBase {
    private MovementAgent _movementAgent;
    private IntervaledAction _repairAction;
    private ResourceManager _resourceManager;
    private Mortal _target;

    private void Awake() {
        _resourceManager = ResourceManager.Instance;
        _movementAgent = GetComponent<MovementAgent>();
        if (_repairAction == null) _repairAction = gameObject.AddComponent<IntervaledAction>();

        _repairAction.interval = 1;
        _repairAction.initialDelay = true;
        _repairAction.action = () => {
            if (!_resourceManager.Pay(new Cost {cash = 10})) return false;

            _target.TakeDamage(-10);
            return true;
        };
    }

    public override void AbandonTask() {
        Destroy(_repairAction);
        base.AbandonTask();
    }

    public override bool Init(Task task) {
        _target = task.payload.GetComponent<Mortal>();
        _target.onDeath.AddListener(Complete);
        _movementAgent.SetDestination(_target.transform.position);
        _movementAgent.isStopped = false;
        return true;
    }

    public override void Behave() {
        if (_target.Hitpoints.Value == _target.MaxHitpoints) Complete();
    }

    private void Complete() {
        Destroy(_repairAction);
        CompleteTask();
    }

    private void OnTriggerEnter(Collider other) {
        if (_target == null || _target.gameObject != other.gameObject) return;
        _repairAction.IsRunning = true;
        _movementAgent.isStopped = true;
    }

    private void OnTriggerExit(Collider other) {
        if (_target == null || _target.gameObject != other.gameObject) return;
        _repairAction.IsRunning = false;
        _movementAgent.isStopped = false;
    }
}
}