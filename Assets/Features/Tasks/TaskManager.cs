using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Features.Save;
using Features.Units.Robots;
using Grimity.Data;
using Grimity.ScriptableObject;
using Grimity.Singleton;
using MonKey.Extensions;
using UnityEngine;

namespace Features.Tasks {
public class TaskManager : GrimitySingleton<TaskManager>, ISavableComponent<TaskManagerData> {
    public RuntimeGameObjectSet allWorkers;
    private readonly List<BaseTask> _availableTasks = new List<BaseTask>();
    private readonly Dictionary<Worker, WorkerData> _workers = new Dictionary<Worker, WorkerData>();

    private void Awake() {
        UpdateAllWorkerList(allWorkers.Items);
        allWorkers.OnChange += UpdateAllWorkerList;
    }

    private void UpdateAllWorkerList(ReadOnlyCollection<GameObject> items) {
        var updatedWorkers = items.Select(o => o.GetComponent<Worker>())
            .Where(worker => worker != null)
            .ToArray();
        var currentWorkers = _workers.Keys;

        foreach (var newWorker in updatedWorkers.Except(currentWorkers)) {
            newWorker.TaskCompleted += OnTaskCompleted;
            newWorker.TaskAbandoned += OnTaskAbandoned;
            _workers.Add(newWorker, new WorkerData(Optional<BaseTask>.NoValue()));
            FindTaskForWorker(newWorker);
        }

        foreach (var removedWorker in currentWorkers.Except(updatedWorkers)) {
            _workers.Remove(removedWorker);
            removedWorker.TaskCompleted -= OnTaskCompleted;
            removedWorker.TaskAbandoned -= OnTaskAbandoned;
            if (!_workers.ContainsKey(removedWorker)) return;
            var task = _workers[removedWorker].Task;
            if (task.HasValue) {
                Enqueue(task.Value);
            }
        }
    }

    public void Enqueue(BaseTask task) {
        if (FindWorkerForTask(task) == TaskResponse.Declined) {
            _availableTasks.Add(task);
        }
    }

    private void OnTaskCompleted(Worker worker, BaseTask task) {
        Debug.Log($"Worker={worker.getSaveID()} completed task={task.Type}");
        FreeWorker(worker);
        FindTaskForWorker(worker);
    }

    private void OnTaskAbandoned(Worker worker, BaseTask baseTask) {
        FreeWorker(worker);
        FindTaskForWorker(worker);
        Enqueue(baseTask);
    }

    private void FreeWorker(Worker worker) {
        var data = new WorkerData(Optional<BaseTask>.NoValue());
        _workers[worker] = data;
    }

    private TaskResponse FindWorkerForTask(BaseTask task) {
        var worker = _workers
            .Where(pair => !pair.Value.Task.HasValue)
            .Select(pair => pair.Key)
            .FirstOrDefault(wrker => wrker.TypePriority.Contains(task.Type));
        return worker == null ? TaskResponse.Declined : AssignTaskToWorker(task, worker);
    }

    private void FindTaskForWorker(Worker worker) {
        var taskResponse = TaskResponse.Declined;
        var declinedTasks = _workers[worker].DeclinedTasks;
        while (taskResponse != TaskResponse.Accepted) {
            var availableTasks = _availableTasks.Where(t => !declinedTasks.Contains(t)).ToArray();
            if (availableTasks.IsEmpty()) return;
            var task = availableTasks.First(baseTask => worker.TypePriority.Contains(baseTask.Type));
            taskResponse = AssignTaskToWorker(task, worker);
        }
    }

    private TaskResponse AssignTaskToWorker(BaseTask task, Worker worker) {
        var taskResponse = worker.SetTask(task);
        var workerData = _workers[worker];
        switch (taskResponse) {
            case TaskResponse.Accepted:
                Debug.Log($"Worker={worker.getSaveID()} accepted task={task.Type}");
                workerData.Task = task.AsOptional();
                _availableTasks.Remove(task);
                break;
            case TaskResponse.Completed:
                Debug.Log($"Worker={worker.getSaveID()} completed instantly task={task.Type}");
                _availableTasks.Remove(task);
                break;
            case TaskResponse.Declined:
                Debug.Log($"Worker={worker.getSaveID()} declined task={task.Type}");
                workerData.DeclinedTasks.Add(task);
                break;
        }

        return taskResponse;
    }

    #region Save

    public string SaveKey => "TaskManager";

    public TaskManagerData Save() {
        string SerialiseWorkerData(WorkerData data) {
            return !data.Task.HasValue ? null : TaskSerializationUtil.SerializeTask(data.Task.Value);
        }

        var assignedTasks = _workers
            .Where(pair => pair.Value.Task.HasValue)
            .ToDictionary(
                tuple => tuple.Key.getSaveID(),
                tuple => SerialiseWorkerData(tuple.Value));
        return new TaskManagerData {
            AvailableTasks =
                _availableTasks.Select(TaskSerializationUtil.SerializeTask).ToList(),
            AssignedTasks = assignedTasks
        };
    }

    public void Load(TaskManagerData data, IReadOnlyDictionary<string, GameObject> objects) {
        _availableTasks.Clear();
        _availableTasks.AddRange(
            data.AvailableTasks.Select(json => TaskSerializationUtil.LoadTask(json, objects)));
        var loadedAssignedTasks = data.AssignedTasks.ToDictionary(
            pair => objects.getBySaveID(pair.Key).GetComponent<Worker>(),
            pair => TaskSerializationUtil.LoadTask(pair.Value, objects));
        foreach (var (worker, baseTask) in loadedAssignedTasks) {
            if (!_workers.ContainsKey(worker)) {
                Debug.LogWarning("Trying to load task for a worker that's not here!");
                continue;
            }

            if (_workers[worker].Task.HasValue) {
                Debug.LogWarning("Loading a task for a worker that already has a task!");
            }

            AssignTaskToWorker(baseTask, worker);
        }
    }

    #endregion
}

public struct TaskManagerData {
    public Dictionary<string, string> AssignedTasks;
    public List<string> AvailableTasks;
}
}