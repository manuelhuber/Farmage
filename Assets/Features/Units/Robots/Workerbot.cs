using System.Collections;
using System.Collections.Generic;
using Features.Queue;
using UnityEngine;

namespace Features.Units.Robots {
public class Workerbot : MonoBehaviour {
    [SerializeField] private JobMultiQueue jobQueue = null;
    [SerializeField] private TaskType[] typePriority = new TaskType[0];

    private Task _currentTask;
    private UnitBehaviourBase _activeBehaviour;

    private readonly Dictionary<TaskType, UnitBehaviourBase>
        _behaviours = new Dictionary<TaskType, UnitBehaviourBase>();

    // Start is called before the first frame update
    private void Awake() {
        // When duplicating a object during play the active behaviour isn't null on Awake...
        _activeBehaviour = null;
        _behaviours[TaskType.Loot] = gameObject.GetComponent<LootGatherer>();
        _behaviours[TaskType.Harvest] = gameObject.GetComponent<HarvestBehaviour>();
        _behaviours[TaskType.Repair] = gameObject.GetComponent<RepairBehaviour>();
        foreach (var unitBehaviourBase in _behaviours.Values) {
            unitBehaviourBase.enabled = false;
        }
    }

    private void Start() {
        StartCoroutine(GetNewTask());
    }

    // Update is called once per frame
    private void Update() {
        if (_activeBehaviour != null) _activeBehaviour.Behave();
    }

    private IEnumerator GetNewTask() {
        while (_activeBehaviour == null) {
            var position = transform.position;
            foreach (var type in typePriority) {
                var task = jobQueue.Dequeue(type,
                    t => Vector3.Distance(t.payload.transform.position, position));

                var newBehaviour = _behaviours[type];
                if (task == null) continue;
                Debug.Log($"{transform.name} dequeued task {task}");

                if (!newBehaviour.Init(task.Value)) continue;
                _currentTask = task.Value;
                _activeBehaviour = newBehaviour;
                _activeBehaviour.enabled = true;
                _activeBehaviour.TaskCompleted.AddListener(() => ResetBehaviour(false));
                _activeBehaviour.TaskAbandoned.AddListener(() => ResetBehaviour(true));
                yield break;
            }

            yield return new WaitForSeconds(.5f);
        }
    }


    private void ResetBehaviour(bool requeue) {
        if (_activeBehaviour != null) {
            _activeBehaviour.TaskCompleted.RemoveAllListeners();
            _activeBehaviour.TaskAbandoned.RemoveAllListeners();
            _activeBehaviour.enabled = false;
            _activeBehaviour = null;
        }

        if (requeue) jobQueue.Enqueue(_currentTask);

        StartCoroutine(GetNewTask());
    }
}
}