using System;
using System.Collections.Generic;
using System.Linq;
using Features.Save;
using Grimity.Collections;
using JetBrains.Annotations;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;

namespace Features.Queue {
[CreateAssetMenu(menuName = "queue/multi queue")]
public class JobMultiQueue : ScriptableObject, ISavableComponent {
    public readonly Dictionary<TaskType, List<BaseTask>> Tasks = new Dictionary<TaskType, List<BaseTask>>();

    public string SaveKey => "JobMultiQueue";

    public string Save() {
        return Tasks.ToJson();
    }

    public void Load(string rawData, IReadOnlyDictionary<string, GameObject> objects) {
        throw new NotImplementedException();
    }

    public int Count(TaskType type) {
        return Tasks.GetOrDefault(type, new List<BaseTask>()).Count;
    }

    public void Enqueue(BaseTask task) {
        Tasks.GetOrCompute(task.type, type => new List<BaseTask>()).Add(task);
    }

    [CanBeNull]
    public T Dequeue<T>(TaskType type, Func<BaseTask, float> prioritisation = null)
        where T : BaseTask {
        if (!Tasks.TryGetValue(type, out var tasks) || tasks.Count == 0) return null;
        var task = prioritisation == null ? tasks[0] : tasks.OrderBy(prioritisation).First();
        tasks.Remove(task);
        return task as T;
    }
}
}