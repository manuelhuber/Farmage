using System;
using Features.Queue;
using Features.Resources;
using Grimity.Actions;
using UnityEngine;
using UnityEngine.AI;

namespace Features.Units.Robots {
public class RepairBehaviour : UnitBehaviourBase {
    private Mortal _target;
    private NavMeshAgent _navMeshAgent;
    private IntervaledAction _repairAction;
    private ResourceManager _resourceManager;

    private void Start() {
        _resourceManager = ResourceManager.Instance;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        if (_repairAction == null) {
            _repairAction = gameObject.AddComponent<IntervaledAction>();
        }

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
        Start();
        _target = task.payload.GetComponent<Mortal>();
        _target.onDeath.AddListener(Complete);
        _navMeshAgent.SetDestination(_target.transform.position);
        _navMeshAgent.isStopped = false;
        return true;
    }

    public override void Behave() {
        if (_target.Hitpoints == _target.MaxHitpoints) Complete();
    }

    private void Complete() {
        Destroy(_repairAction);
        CompleteTask();
    }

    private void OnTriggerEnter(Collider other) {
        if (_target == null || _target.gameObject != other.gameObject) return;
        _repairAction.IsRunning = true;
        _navMeshAgent.isStopped = true;
    }

    private void OnTriggerExit(Collider other) {
        if (_target == null || _target.gameObject != other.gameObject) return;
        _repairAction.IsRunning = false;
        _navMeshAgent.isStopped = false;
    }
}
}