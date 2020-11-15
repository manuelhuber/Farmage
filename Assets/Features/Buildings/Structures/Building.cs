using Features.Buildings.BuildMenu;
using Features.Buildings.UI;
using Features.Health;
using Features.Tasks;
using UnityEngine;

namespace Features.Buildings.Structures {
public class Building : MonoBehaviour {
    public BuildingMenuEntry menuEntry;
    private readonly bool _autoRepair = false;
    private bool _waitingForRepair;

    private void Start() {
        BuildingManager.Instance.RegisterNewBuilding(this);
        if (_autoRepair) {
            RegisterAutoRepair();
        }
    }

    private void RegisterAutoRepair() {
        var mortal = GetComponent<Mortal>();
        mortal.Hitpoints.OnChange(hitpoints => {
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