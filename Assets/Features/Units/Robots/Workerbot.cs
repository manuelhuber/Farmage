using System;
using System.Collections;
using System.Collections.Generic;
using Features.Queue;
using Grimity.ScriptableObject;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Features.Units.Robots {
public class Workerbot : MonoBehaviour {
    private readonly Dictionary<TaskType, UnitBehaviourBase>
        _behaviours = new Dictionary<TaskType, UnitBehaviourBase>();

    private Task _currentTask;
    [SerializeField] private JobMultiQueue _jobQueue;

    private Func<bool, UnityAction> _resetBehaviour;
    [SerializeField] private TaskType[] _typePriority;
    [FormerlySerializedAs("active")] public UnitBehaviourBase activeBehaviour;
    [SerializeField] private RuntimeGameObjectSet buildings;

    // Start is called before the first frame update
    private void Start() {
        var lootGatherer = gameObject.AddComponent<LootGatherer>();
        lootGatherer.buildings = buildings;
        _behaviours[TaskType.Loot] = lootGatherer;
        _behaviours[TaskType.Harvest] = gameObject.AddComponent<HarvestBehaviour>();
        _behaviours[TaskType.Repair] = gameObject.AddComponent<RepairBehaviour>();
        foreach (var unitBehaviourBase in _behaviours.Values
        ) // TODO disable them in the next frame so their "start" code runs?
            unitBehaviourBase.enabled = false;

        _resetBehaviour = requeue => () => {
            if (activeBehaviour != null) {
                activeBehaviour.TaskCompleted.RemoveAllListeners();
                activeBehaviour.TaskAbandoned.RemoveAllListeners();
                activeBehaviour.enabled = false;
                activeBehaviour = null;
            }

            if (requeue) _jobQueue.Enqueue(_currentTask);

            StartCoroutine(GetNewTask());
        };
        _resetBehaviour(false)();
    }

    // Update is called once per frame
    private void Update() {
        if (activeBehaviour != null) activeBehaviour.Behave();
    }

    private IEnumerator GetNewTask() {
        while (activeBehaviour == null) {
            var position = transform.position;
            foreach (var type in _typePriority) {
                var task = _jobQueue.Dequeue(type, t => Vector3.Distance(t.payload.transform.position, position));

                var newBehaviour = _behaviours[type];
                if (task == null) continue;

                if (!newBehaviour.Init(task.Value)) continue;
                _currentTask = task.Value;
                activeBehaviour = newBehaviour;
                activeBehaviour.enabled = true;
                activeBehaviour.TaskCompleted.AddListener(_resetBehaviour(false));
                activeBehaviour.TaskAbandoned.AddListener(_resetBehaviour(true));
                yield break;
            }

            yield return new WaitForSeconds(.5f);
        }
    }
}
}