using System.Linq;
using Features.Building.Structures.WheatField;
using Features.Queue;
using Features.Resources;
using Grimity.ScriptableObject;
using UnityEngine;
using UnityEngine.AI;

namespace Features.Units.Robots {
public class HarvestBehaviour : WokerbotBehaviourBase {
    [SerializeField] private JobMultiQueue _queue;
    [SerializeField] private RuntimeGameObjectSet _buildings;

    private GameObject target;
    private NavMeshAgent _navMeshAgent;
    private ResourceManager _resourceManager;

    private void Start() {
        _resourceManager = ResourceManager.Instance;

        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public override int ActivationPriority() {
        return _queue.Count(TaskType.Harvest) > 0 ? 100 : 0;
    }

    public override void Behave() {
        if (target == null) {
            var task = _queue.Dequeue(TaskType.Harvest);
            if (task == null) {
                completeTask();
                return;
            }

            target = task.Value.payload;
            _navMeshAgent.SetDestination(target.transform.position);
        }

        if (!(_navMeshAgent.remainingDistance < 1)) return;
        var harvest = target.GetComponent<WheatField>().harvest();
        _resourceManager.Add(new Cost {cash = harvest});
        target = null;
        completeTask();
    }

    public override void Abandon() {
    }
}
}