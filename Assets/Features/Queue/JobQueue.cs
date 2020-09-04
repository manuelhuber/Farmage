using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Features.Queue {
[CreateAssetMenu(menuName = "queue/simple")]
public class JobQueue : ScriptableObject {
    public readonly List<BaseTask> Tasks = new List<BaseTask>();

    public int Count() {
        return Tasks.Count;
    }

    public void Enqueue(BaseTask task) {
        Tasks.Add(task);
    }

    [CanBeNull]
    public BaseTask Dequeue(Func<BaseTask, float> prioritisation = null) {
        if (Tasks.Count == 0) return null;
        var task = prioritisation == null ? Tasks[0] : Tasks.OrderBy(prioritisation).First();
        Tasks.Remove(task);
        return task;
    }
}
}