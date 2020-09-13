using System;

namespace Features.Tasks {
[Serializable]
public abstract class BaseTask {
    public abstract TaskType Type { get; }

    public override string ToString() {
        return $"type={Type}";
    }
}
}