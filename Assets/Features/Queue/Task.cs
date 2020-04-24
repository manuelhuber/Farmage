using System;
using UnityEngine;

namespace Features.Queue {
[Serializable]
public struct Task {
    public TaskType type;
    public GameObject payload;
}

[Serializable]
public enum TaskType {
    Loot,
    Harvest,
    Repair
}
}