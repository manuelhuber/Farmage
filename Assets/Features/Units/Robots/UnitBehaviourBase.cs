using System;
using Features.Queue;
using UnityEngine;
using UnityEngine.Events;

namespace Features.Units.Robots {
public abstract class UnitBehaviourBase : MonoBehaviour, IUnitBehaviour {
    public UnityEvent TaskCompleted { get; } = new UnityEvent();
    public UnityEvent TaskAbandoned { get; } = new UnityEvent();
    private IUnitBehaviour _unitBehaviourImplementation;

    public abstract bool Init(Task task);

    public virtual void Behave() {
        ;
    }

    public virtual void AbandonTask() {
        TaskAbandoned.Invoke();
    }

    protected void CompleteTask() {
        TaskCompleted.Invoke();
    }

    private void OnDestroy() {
        AbandonTask();
    }
}
}