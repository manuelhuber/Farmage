using Features.Health;
using Features.Queue;
using Unity.Mathematics;
using UnityEngine;

namespace Features.Building.Structures {
public class Building : MonoBehaviour {
    public int2 size;
    [SerializeField] private JobMultiQueue queue = null;
    private bool _waitingForRepair;

    // Start is called before the first frame update
    private void Start() {
        var mortal = GetComponent<Mortal>();
        mortal.Hitpoints.OnChange(hitpoints => {
            if (hitpoints == mortal.MaxHitpoints) {
                _waitingForRepair = false;
                return;
            }

            if (_waitingForRepair) return;
            queue.Enqueue(new Task {type = TaskType.Repair, payload = gameObject});
            _waitingForRepair = true;
        });
    }
}
}