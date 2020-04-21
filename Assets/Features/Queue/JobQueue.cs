using System;
using System.Collections.Generic;
using System.Linq;
using Grimity.Collections;
using Grimity.Singleton;
using UnityEngine;

namespace Features.Queue {
[CreateAssetMenu(menuName = "queue/simple")]
public class JobQueue : ScriptableObject {
    public readonly List<Task> Tasks = new List<Task>();

    public int Count() {
        return Tasks.Count;
    }

    public void Enqueue(Task task) {
        Tasks.Add(task);
    }

    public Task? Dequeue(Func<Task, float> prioritisation = null) {
        if (Tasks.Count == 0) return null;
        var task = prioritisation == null ? Tasks[0] : Tasks.OrderBy(prioritisation).First();
        Tasks.Remove(task);
        return task;
    }
}
}