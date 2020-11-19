using UnityEngine;

namespace Features.Tasks {
public class SimpleTask : BaseTask {
    public GameObject Payload;

    public SimpleTask(GameObject payload, TaskType taskType) {
        Payload = payload;
        Type = taskType;
    }

    public static string SaveKeyStatic => "SimpleTask";
    public override TaskType Type { get; }
}
}