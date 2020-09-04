using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Features.Queue;
using Features.Save;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;

namespace Features.Units.Robots {
public class Workerbot : MonoBehaviour, ISavableComponent {
    [SerializeField] private JobMultiQueue jobQueue;
    [SerializeField] private TaskType[] typePriority = new TaskType[0];

    private readonly Dictionary<TaskType, IUnitBehaviourBase<BaseTask>>
        _behaviours = new Dictionary<TaskType, IUnitBehaviourBase<BaseTask>>();

    private IUnitBehaviourBase<BaseTask> _activeBehaviour;

    private BaseTask _currentTask;

    // Start is called before the first frame update
    private void Awake() {
        // When duplicating a object during play the active behaviour isn't null on Awake...
        _activeBehaviour = null;
        IUnitBehaviourBase<SimpleTask> lootGatherer = gameObject.GetComponent<LootGatherer>();
        _behaviours[TaskType.Loot] = lootGatherer;
        _behaviours[TaskType.Harvest] = gameObject.GetComponent<HarvestBehaviour>();
        _behaviours[TaskType.Repair] = gameObject.GetComponent<RepairBehaviour>();
        _behaviours[TaskType.Build] = gameObject.GetComponent<BuildBehaviour>();
    }

    private void Start() {
        StartCoroutine(GetNewTask());
    }

    // Update is called once per frame
    private void Update() {
        if (_activeBehaviour != null) _activeBehaviour.Behave();
    }

    public string SaveKey => "workerbot";

    public string Save() {
        if (_activeBehaviour == null) return "";
        var activeType = _behaviours.FirstOrDefault(pair => pair.Value == _activeBehaviour).Key;
        // TODO serialise current task
        return new WorkerbotData {active = activeType}.ToJson();
    }

    public void Load(string rawData, IReadOnlyDictionary<string, GameObject> objects) {
        if (rawData == "") return;
        var data = rawData.FromJson<WorkerbotData>();
        // TODO deserialise current task
        SetTask(new SimpleTask {type = data.active});
    }

    private IEnumerator GetNewTask() {
        while (_activeBehaviour == null) {
            foreach (var type in typePriority) {
                var task = jobQueue.Dequeue<BaseTask>(type);
                if (task == null) continue;
                if (!SetTask(task)) continue;
                yield break;
            }

            yield return new WaitForSeconds(.5f);
        }
    }

    private bool SetTask(BaseTask task) {
        var newBehaviour = _behaviours[task.type];
        if (!newBehaviour.Init(task)) {
            jobQueue.Enqueue(task);
            return false;
        }

        _currentTask = task;
        _activeBehaviour = newBehaviour;
        _activeBehaviour.TaskCompleted.AddListener(() => ResetBehaviour(false));
        _activeBehaviour.TaskAbandoned.AddListener(() => ResetBehaviour(true));
        return true;
    }


    private void ResetBehaviour(bool requeueCurrentTask) {
        if (_activeBehaviour != null) {
            _activeBehaviour.TaskCompleted.RemoveAllListeners();
            _activeBehaviour.TaskAbandoned.RemoveAllListeners();
            _activeBehaviour = null;
        }

        if (requeueCurrentTask) jobQueue.Enqueue(_currentTask);
        StartCoroutine(GetNewTask());
    }
}

[Serializable]
internal struct WorkerbotData {
    public TaskType active;
}
}