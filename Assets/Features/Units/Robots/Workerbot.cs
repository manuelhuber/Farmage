using System.Collections;
using System.Collections.Generic;
using Features.Units.Robots;
using UnityEngine;

public class Workerbot : MonoBehaviour {
    public WokerbotBehaviour active;
    private WokerbotBehaviour[] _wokerbotBehaviours;

    // Start is called before the first frame update
    void Start() {
        _wokerbotBehaviours = GetComponents<WokerbotBehaviour>();
    }

    // Update is called once per frame
    void Update() {
        if (active == null) {
            foreach (var wokerbotBehaviour in _wokerbotBehaviours) {
                active = wokerbotBehaviour;
            }
        }

        active.Behave();
    }
}