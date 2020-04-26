using Features.Queue;
using UnityEngine.Events;

namespace Features.Units.Robots {
public interface IUnitBehaviour {
    UnityEvent TaskCompleted { get; }
    UnityEvent TaskAbandoned { get; }
    bool Init(Task task);
    void Behave();
    void AbandonTask();
}
}