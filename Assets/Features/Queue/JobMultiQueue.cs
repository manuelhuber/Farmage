using System;
using System.Collections.Generic;
using System.Linq;
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

    public Task? Dequeue(TaskType type, Func<Task, float> prioritisation = null) {
        if (!Tasks.TryGetValue(type, out var tasks) || tasks.Count == 0) return null;
        var task = prioritisation == null ? tasks[0] : tasks.OrderBy(prioritisation).First();
        tasks.Remove(task);
        return task;
    }
}
}