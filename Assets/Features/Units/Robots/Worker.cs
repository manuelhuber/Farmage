using System;
using System.Collections.Generic;
using Features.Tasks;
using Grimity.Collections;
using Grimity.Data;
using UnityEngine;

namespace Features.Units.Robots {
public class Worker : MonoBehaviour {
    [SerializeField] private TaskType[] typePriority = new TaskType[0];
    public TaskType[] TypePriority => typePriority;

    public BaseTask CurrentTask { get; private set; }

    private readonly Dictionary<TaskType, IUnitBehaviourBase<BaseTask>>
        _behaviours = new Dictionary<TaskType, IUnitBehaviourBase<BaseTask>>();

    private readonly List<Collider> _inRange = new List<Collider>();

    private readonly Observable<Collider[]> _inRangeObservable =
        new Observable<Collider[]>(new Collider[] { });

    private IUnitBehaviourBase<BaseTask> _activeBehaviour;

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

    private void OnTriggerEnter(Collider other) {
        _inRange.Add(other);
        _inRange.RemoveAll(obj => obj == null);
        _inRangeObservable.Set(_inRange.ToArray());
    }

    private void OnTriggerExit(Collider other) {
        _inRange.Remove(other);
        _inRange.RemoveAll(obj => obj == null);
        _inRangeObservable.Set(_inRange.ToArray());
    }

    public event Action<Worker, BaseTask> TaskCompleted;
    public event Action<Worker, BaseTask> TaskAbandoned;


    public TaskResponse SetTask(BaseTask task) {
        var newBehaviour = _behaviours.GetOrDefault(task.Type, null);
        if (newBehaviour == null) {
            return TaskResponse.Declined;
        }

        var taskResponse = newBehaviour.Init(task, _inRangeObservable);

        if (taskResponse == TaskResponse.Accepted) {
            newBehaviour.TaskCompleted += OnTaskCompleted;
            newBehaviour.TaskAbandoned += OnTaskAbandoned;
            CurrentTask = task;
            _activeBehaviour = newBehaviour;
        }

        return taskResponse;
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
}
}