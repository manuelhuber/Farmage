using System;
using UnityEngine.Events;

namespace Features.Units.Robots {
public interface WokerbotBehaviour {
    int ActivationPriority();
    void Behave();
    void Abandon();
    void onTaskComplete(UnityAction unityAction);
    void removeOnTaskComplete(UnityAction unityAction);
}
}