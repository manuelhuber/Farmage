using System.Linq;
using Features.Units.Robots;
using UnityEngine;
using UnityEngine.Events;

public class Workerbot : MonoBehaviour {
    public WokerbotBehaviour active;
    private WokerbotBehaviour[] _wokerbotBehaviours;

    private UnityAction _resetBehaviour;

    // Start is called before the first frame update
    void Start() {
        _wokerbotBehaviours = GetComponents<WokerbotBehaviour>();
        _resetBehaviour = () => {
            active.removeOnTaskComplete(_resetBehaviour);
            active = null;
        };
    }

    // Update is called once per frame
    void Update() {
        if (active == null) {
            active = _wokerbotBehaviours.OrderByDescending(behaviour => behaviour.ActivationPriority() + 1).First();
            active.onTaskComplete(_resetBehaviour);
        }

        active.Behave();
    }
}