using System;
using Features.Queue;
using UnityEngine.Events;

namespace Features.Units.Robots {
public interface IUnitBehaviour {
    bool Init(Task task);
    void Behave();
    void AbandonTask();
    UnityEvent TaskCompleted { get; }
    UnityEvent TaskAbandoned { get; }
}
}