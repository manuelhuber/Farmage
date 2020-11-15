using Features.Health;
using Features.Tasks;
using UnityEngine;

namespace Features.Building.Structures {
public class Building : MonoBehaviour {
    private readonly bool _autoRepair = false;
    private bool _waitingForRepair;

    private void Start() {
        var mortal = GetComponent<Mortal>();
        mortal.Hitpoints.OnChange(hitpoints => {
            if (!_autoRepair) return;
            if (hitpoints == mortal.MaxHitpoints) {
                _waitingForRepair = false;
                return;
            }

            if (_waitingForRepair) return;
            TaskManager.Instance.Enqueue(new SimpleTask(gameObject, TaskType.Repair));
            _waitingForRepair = true;
        });
    }
}
}