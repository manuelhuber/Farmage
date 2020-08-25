using System;
using UnityEngine;

namespace Features.Queue {
[Serializable]
public struct Task {
    public TaskType type;
    public GameObject payload;

    public override string ToString() {
        return $"type={type} payload={payload.name}";
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