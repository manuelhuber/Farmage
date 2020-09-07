using System;
using Features.Tasks;
using UnityEngine;

namespace Features.Units.Robots {
public abstract class UnitBehaviourBase<T> : MonoBehaviour, IUnitBehaviourBase<T> where T : BaseTask {
    private void OnDestroy() {
        AbandonTask();
    }

    public event Action<IUnitBehaviourBase<T>> TaskCompleted;
    public event Action<IUnitBehaviourBase<T>> TaskAbandoned;

    public bool Init(BaseTask task) {
        return InitImpl(task as T);
    }

    public virtual void Behave() {
    }

    public virtual void AbandonTask() {
        TaskAbandoned?.Invoke(this);
    }

    public void CompleteTask() {
        TaskCompleted?.Invoke(this);
    }

    protected abstract bool InitImpl(T task);
}

public interface IUnitBehaviourBase<out T> where T : BaseTask {
    event Action<IUnitBehaviourBase<T>> TaskCompleted;
    event Action<IUnitBehaviourBase<T>> TaskAbandoned;
    bool Init(BaseTask task);
    void Behave();
    void AbandonTask();
    void CompleteTask();
}
}