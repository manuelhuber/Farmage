using System;

namespace Features.Queue {
[Serializable]
public class BaseTask {
    public TaskType type;

    public override string ToString() {
        return $"type={type}";
    }
}

[Serializable]
public enum TaskType {
    Loot,
    Harvest,
    Repair,
    Build
}
}