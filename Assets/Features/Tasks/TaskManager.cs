using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Features.Units.Robots;
using Grimity.Data;
using Grimity.ScriptableObject;
using Grimity.Singleton;
using MonKey.Extensions;
using UnityEngine;

namespace Features.Tasks {
public class TaskManager : GrimitySingleton<TaskManager> {
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
        if (!FindWorkerForTask(task)) {
            _availableTasks.Add(task);
        }
    }

    private void OnTaskCompleted(Worker worker, BaseTask baseTask) {
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

    private bool FindWorkerForTask(BaseTask task) {
        var worker = _workers.Where(pair => !pair.Value.Task.HasValue)
            .Select(pair => pair.Key)
            .FirstOrDefault();
        if (worker == null) return false;
        return AssignTaskToWorker(task, worker);
    }

    private void FindTaskForWorker(Worker worker) {
        if (_availableTasks.IsEmpty()) return;
        var task = _availableTasks.First();
        AssignTaskToWorker(task, worker);
    }

    private bool AssignTaskToWorker(BaseTask task, Worker worker) {
        if (!worker.SetTask(task)) return false;
        _workers[worker] = new WorkerData(Optional<BaseTask>.Of(task));
        _availableTasks.Remove(task);
        return true;
    }

    #region Save

    private readonly struct WorkerData {
        public readonly Optional<BaseTask> Task;

        public WorkerData(Optional<BaseTask> task) {
            Task = task;
        }
    }

    #endregion
}
}