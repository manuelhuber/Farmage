using Features.Queue;
using UnityEngine;
using UnityEngine.Events;

namespace Features.Units.Robots {
public abstract class UnitBehaviourBase<T> : MonoBehaviour, IUnitBehaviourBase<T> where T : BaseTask {
    private void OnDestroy() {
        AbandonTask();
    }

    public UnityEvent TaskCompleted { get; } = new UnityEvent();
    public UnityEvent TaskAbandoned { get; } = new UnityEvent();

    public bool Init(BaseTask task) {
        return InitImpl(task as T);
    }

    public virtual void Behave() {
    }

    public virtual void AbandonTask() {
        TaskAbandoned.Invoke();
    }

    public void CompleteTask() {
        TaskCompleted.Invoke();
    }

    protected abstract bool InitImpl(T task);
}

public interface IUnitBehaviourBase<out T> where T : BaseTask {
    UnityEvent TaskCompleted { get; }
    UnityEvent TaskAbandoned { get; }
    bool Init(BaseTask task);
    void Behave();
    void AbandonTask();
    void CompleteTask();
}
}