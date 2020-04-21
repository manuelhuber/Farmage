using System.Collections.Generic;
using Grimity.Collections;
using UnityEngine;

namespace Features.Queue {
[CreateAssetMenu(menuName = "queue/multi queue")]
public class JobMultiQueue : ScriptableObject {
    public readonly Dictionary<TaskType, List<Task>> Tasks = new Dictionary<TaskType, List<Task>>();

    public int Count(TaskType type) {
        return Tasks.GetOrDefault(type, new List<Task>()).Count;
    }

    public void Enqueue(Task task) {
        Tasks.GetOrCompute(task.type, type => new List<Task>()).Add(task);
    }

    public Task? Dequeue(TaskType type) {
        if (!Tasks.TryGetValue(type, out var tasks) || tasks.Count == 0) return null;
        var task = tasks[0];
        tasks.Remove(task);
        return task;
    }
}
}