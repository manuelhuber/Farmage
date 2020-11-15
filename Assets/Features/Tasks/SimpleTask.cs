using System.Collections.Generic;
using UnityEngine;

namespace Features.Tasks {
public class SimpleTask : BaseTask {
    private TaskType _type;

    public GameObject Payload;

    public SimpleTask(GameObject payload, TaskType taskType) {
        Payload = payload;
        _type = taskType;
    }

    public static string SaveKeyStatic => "SimpleTask";
    public override TaskType Type => _type;
}
}