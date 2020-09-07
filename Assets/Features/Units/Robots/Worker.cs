using System;
using System.Collections.Generic;
using System.Linq;
using Features.Save;
using Features.Tasks;
using Grimity.Collections;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;

namespace Features.Units.Robots {
public class Worker : MonoBehaviour, ISavableComponent {
    [SerializeField] private TaskType[] typePriority = new TaskType[0];

    private readonly Dictionary<TaskType, IUnitBehaviourBase<BaseTask>>
        _behaviours = new Dictionary<TaskType, IUnitBehaviourBase<BaseTask>>();

    private IUnitBehaviourBase<BaseTask> _activeBehaviour;

    public BaseTask CurrentTask { get; private set; }

    // Start is called before the first frame update
    private void Awake() {
        // When duplicating a object during play the active behaviour isn't null on Awake...
        _activeBehaviour = null;
        _behaviours[TaskType.Deliver] = gameObject.GetComponent<DeliveryBehaviour>();
        _behaviours[TaskType.Harvest] = gameObject.GetComponent<HarvestBehaviour>();
        _behaviours[TaskType.Repair] = gameObject.GetComponent<RepairBehaviour>();
        _behaviours[TaskType.Build] = gameObject.GetComponent<BuildBehaviour>();
    }

    private void Update() {
        _activeBehaviour?.Behave();
    }

    public event Action<Worker, BaseTask> TaskCompleted;
    public event Action<Worker, BaseTask> TaskAbandoned;


    public bool SetTask(BaseTask task) {
        var newBehaviour = _behaviours.GetOrDefault(task.type, null);
        if (newBehaviour == null || !newBehaviour.Init(task)) {
            return false;
        }

        CurrentTask = task;
        _activeBehaviour = newBehaviour;
        newBehaviour.TaskCompleted += OnTaskCompleted;
        newBehaviour.TaskAbandoned += OnTaskAbandoned;

        return true;
    }


    private void ResetBehaviour() {
        CurrentTask = null;
        _activeBehaviour.TaskCompleted -= OnTaskCompleted;
        _activeBehaviour.TaskAbandoned -= OnTaskAbandoned;
        _activeBehaviour = null;
    }

    private void OnTaskCompleted(IUnitBehaviourBase<BaseTask> unitBehaviourBase) {
        if (unitBehaviourBase != _activeBehaviour) {
            Debug.LogWarning("Receiving OnTaskCompleted from inactive behaviour!");
        }

        var currentTask = CurrentTask;
        ResetBehaviour();
        TaskCompleted?.Invoke(this, currentTask);
    }

    private void OnTaskAbandoned(IUnitBehaviourBase<BaseTask> unitBehaviourBase) {
        if (unitBehaviourBase != _activeBehaviour) {
            Debug.LogWarning("Receiving OnTaskAbandoned from inactive behaviour!");
        }

        var currentTask = CurrentTask;
        ResetBehaviour();
        TaskAbandoned?.Invoke(this, currentTask);
    }

    #region Save

    public string SaveKey => "workerbot";

    public string Save() {
        if (_activeBehaviour == null) return "";
        var activeType = _behaviours.FirstOrDefault(pair => pair.Value == _activeBehaviour).Key;
        // TODO serialise current task
        return new WorkerData {active = activeType}.ToJson();
    }

    public void Load(string rawData, IReadOnlyDictionary<string, GameObject> objects) {
        if (rawData == "") return;
        var data = rawData.FromJson<WorkerData>();
        // TODO deserialise current task
        SetTask(new SimpleTask {type = data.active});
    }

    [Serializable]
    private struct WorkerData {
        public TaskType active;
    }

    #endregion
}
}