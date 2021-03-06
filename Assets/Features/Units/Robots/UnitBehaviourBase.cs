using System;
using Features.Tasks;
using Grimity.Data;
using UnityEngine;

namespace Features.Units.Robots {
public abstract class UnitBehaviourBase<T> : MonoBehaviour, IUnitBehaviourBase<T> where T : BaseTask {
    private void OnDestroy() {
        AbandonTask();
    }

    public event Action<IUnitBehaviourBase<T>> TaskCompleted;
    public event Action<IUnitBehaviourBase<T>> TaskAbandoned;

    public TaskResponse Init(BaseTask task, Observable<Collider[]> inRange) {
        return InitImpl(task as T, inRange);
    }

    public virtual void Behave() {
    }

    public virtual void AbandonTask() {
        TaskAbandoned?.Invoke(this);
    }

    public void CompleteTask() {
        TaskCompleted?.Invoke(this);
    }

    protected abstract TaskResponse InitImpl(T task, Observable<Collider[]> inRange);
}

public interface IUnitBehaviourBase<out T> where T : BaseTask {
    event Action<IUnitBehaviourBase<T>> TaskCompleted;
    event Action<IUnitBehaviourBase<T>> TaskAbandoned;
    TaskResponse Init(BaseTask task, Observable<Collider[]> inRange);
    void Behave();
    void AbandonTask();
    void CompleteTask();
}
}