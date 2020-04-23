using System;
using UnityEngine;
using UnityEngine.Events;

namespace Features.Units.Robots {
public abstract class WokerbotBehaviourBase : MonoBehaviour, WokerbotBehaviour {
    protected UnityEvent onTaskCompleted = new UnityEvent();
    private WokerbotBehaviour _wokerbotBehaviourImplementation;

    public abstract int ActivationPriority();
    public abstract void Behave();
    public abstract void Abandon();

    protected void completeTask() {
        onTaskCompleted.Invoke();
    }

    public void onTaskComplete(UnityAction unityAction) {
        onTaskCompleted.AddListener(unityAction);
    }

    public void removeOnTaskComplete(UnityAction a) {
        onTaskCompleted.RemoveListener(a);
    }
}
}