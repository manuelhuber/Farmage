using System.Collections;
using System.Collections.Generic;
using Features.Queue;
using Features.Units;
using UnityEngine;

public class Building : MonoBehaviour {
    [SerializeField] private JobMultiQueue _queue;

    private bool waitingForRepair;

    // Start is called before the first frame update
    private void Start() {
        var mortal = GetComponent<Mortal>();
        mortal.onDamage.AddListener(() => {
            if (mortal.Hitpoints == mortal.MaxHitpoints) {
                waitingForRepair = false;
                return;
            }

            if (waitingForRepair) return;
            _queue.Enqueue(new Task {type = TaskType.Repair, payload = gameObject});
            waitingForRepair = true;
        });
    }
}