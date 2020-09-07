using System;

namespace Features.Tasks {
[Serializable]
public class BaseTask {
    public TaskType type;

    public override string ToString() {
        return $"type={type}";
    }
}
}