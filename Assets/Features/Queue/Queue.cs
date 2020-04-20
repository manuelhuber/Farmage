using System.Collections.Generic;
using Grimity.Collections;
using Grimity.Singleton;
using UnityEngine;

namespace Features.Queue {
public enum TaskType {
    Loot
}

public struct Task {
    public TaskType type;
    public GameObject payload;
}

public class Queue : GrimitySingleton<Queue> {
    private static readonly Dictionary<TaskType, List<Task>> Tasks = new Dictionary<TaskType, List<Task>>();

    public static int Count(TaskType type) {
        return Tasks.GetOrDefault(type, new List<Task>()).Count;
    }

    public static void Enqueue(Task task) {
        Tasks.GetOrCompute(task.type, type => new List<Task>()).Add(task);
    }

    public static Task? Dequeue(TaskType type) {
        if (!Tasks.TryGetValue(type, out var tasks) || tasks.Count == 0) return null;
        var task = tasks[0];
        tasks.Remove(task);
        return task;
    }
}
}