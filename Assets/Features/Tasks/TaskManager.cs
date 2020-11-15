using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Features.Common;
using Features.Units.Robots;
using Grimity.Data;
using Grimity.ScriptableObject;
using MonKey.Extensions;
using UnityEngine;

namespace Features.Tasks {
public class TaskManager : Manager<TaskManager> {
    public RuntimeGameObjectSet allWorkers;
    private readonly List<BaseTask> _availableTasks = new List<BaseTask>();
    private readonly List<BaseTask> _cancelledTasks = new List<BaseTask>();
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

    public void CancelTask(BaseTask task) {
        if (task == null || _availableTasks.Remove(task)) return;
        _cancelledTasks.Add(task);
        foreach (var (worker, data) in _workers) {
            if (!data.Task.HasValue || data.Task.Value != task) continue;
            worker.AbandonCurrentTask();
            break;
        }
    }

    private void OnTaskCompleted(Worker worker, BaseTask task) {
        FreeWorker(worker);
        FindTaskForWorker(worker);
    }

    private void OnTaskAbandoned(Worker worker, BaseTask baseTask) {
        FreeWorker(worker);
        FindTaskForWorker(worker);
        var taskWasCancelled = _cancelledTasks.Remove(baseTask);
        if (!taskWasCancelled) {
            Enqueue(baseTask);
        }
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
                workerData.Task = task.AsOptional();
                _availableTasks.Remove(task);
                break;
            case TaskResponse.Completed:
                _availableTasks.Remove(task);
                break;
            case TaskResponse.Declined:
                workerData.DeclinedTasks.Add(task);
                break;
        }

        return taskResponse;
    }
}
}