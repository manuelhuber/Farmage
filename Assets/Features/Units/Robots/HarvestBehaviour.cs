using Features.Building.Structures.WheatField;
using Features.Queue;
using Features.Resources;
using UnityEngine;
using UnityEngine.AI;

namespace Features.Units.Robots {
public class HarvestBehaviour : UnitBehaviourBase {
    private GameObject target;
    private NavMeshAgent _navMeshAgent;
    private ResourceManager _resourceManager;

    private void Start() {
        _resourceManager = ResourceManager.Instance;
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public override bool Init(Task task) {
        Start();
        target = task.payload;
        _navMeshAgent.SetDestination(target.transform.position);
        _navMeshAgent.isStopped = false;
        return true;
    }

    public override void Behave() {
        if (!(_navMeshAgent.remainingDistance < _navMeshAgent.stoppingDistance)) return;
        var harvest = target.GetComponent<WheatField>().harvest();
        _resourceManager.Add(new Cost {cash = harvest});
        CompleteTask();
    }
}
}