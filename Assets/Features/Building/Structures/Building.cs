using Features.Queue;
using Features.Units;
using Unity.Mathematics;
using UnityEngine;

namespace Features.Building.Structures {
public class Building : MonoBehaviour {
    public int2 size;
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
}