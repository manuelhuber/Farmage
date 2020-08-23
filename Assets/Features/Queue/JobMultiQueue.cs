using System;
using System.Collections.Generic;
using System.Linq;
using Features.Save;
using Grimity.Collections;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;

namespace Features.Queue {
[CreateAssetMenu(menuName = "queue/multi queue")]
public class JobMultiQueue : ScriptableObject, ISavableComponent {
    public readonly Dictionary<TaskType, List<Task>> Tasks = new Dictionary<TaskType, List<Task>>();

    public int Count(TaskType type) {
        return Tasks.GetOrDefault(type, new List<Task>()).Count;
    }

    public void Enqueue(Task task) {
        Tasks.GetOrCompute(task.type, type => new List<Task>()).Add(task);
    }

    public Task? Dequeue(TaskType type, Func<Task, float> prioritisation = null) {
        if (!Tasks.TryGetValue(type, out var tasks) || tasks.Count == 0) return new Task?();
        var task = prioritisation == null ? tasks[0] : tasks.OrderBy(prioritisation).First();
        tasks.Remove(task);
        return task;
    }

    public string SaveKey => "JobMultiQueue";

    public string Save() {
        return Tasks.ToJson();
    }

    public void Load(string rawData, IReadOnlyDictionary<string, GameObject> objects) {
        throw new NotImplementedException();
    }
}
}