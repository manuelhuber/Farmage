using System.Collections.Generic;
using Features.Save;
using UnityEngine;

namespace Features.Tasks {
public class SimpleTask : BaseTask, ISavableComponent<SimpleTaskData> {
    private TaskType _type;

    public GameObject Payload;

    public SimpleTask(GameObject payload, TaskType taskType) {
        Payload = payload;
        _type = taskType;
    }

    public static string SaveKeyStatic => "SimpleTask";
    public override TaskType Type => _type;

    public string SaveKey => SaveKeyStatic;

    public SimpleTaskData Save() {
        return new SimpleTaskData(Type, Payload.getSaveID());
    }

    public void Load(SimpleTaskData data, IReadOnlyDictionary<string, GameObject> objects) {
        _type = data.Type;
        Payload = objects.getBySaveID(data.Payload);
    }
}

public readonly struct SimpleTaskData {
    public readonly TaskType Type;
    public readonly string Payload;

    public SimpleTaskData(TaskType type, string payload) {
        Type = type;
        Payload = payload;
    }
}
}