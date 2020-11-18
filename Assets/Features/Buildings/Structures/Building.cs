using Features.Buildings.BuildMenu;
using Features.Buildings.UI;
using Features.Health;
using Features.Tasks;
using Features.Ui.Actions;
using Grimity.Data;
using UnityEngine;

namespace Features.Buildings.Structures {
public class Building : MonoBehaviour, IHasActions {
    [SerializeField] private Sprite RepairIcon;
    public BuildingMenuEntry menuEntry;

    private readonly Observable<ActionEntryData[]> _actions =
        new Observable<ActionEntryData[]>(new ActionEntryData[] { });

    private Mortal _mortal;
    private bool _waitingForRepair;

    private void Start() {
        _mortal = GetComponent<Mortal>();
        BuildingManager.Instance.RegisterNewBuilding(this);
        _mortal.Hitpoints.OnChange(hitpoints => {
            if (hitpoints == _mortal.MaxHitpoints) {
                _waitingForRepair = false;
            }
        });

        _actions.Set(new[] {
            new ActionEntryData {
                Active = true, Image = RepairIcon, OnSelect =
                    () => {
                        if (_waitingForRepair || _mortal.Hitpoints.Value == _mortal.MaxHitpoints) return;
                        TaskManager.Instance.Enqueue(new SimpleTask(gameObject, TaskType.Repair));
                        _waitingForRepair = true;
                    }
            }
        });
    }

    public IObservable<ActionEntryData[]> GetActions() {
        return _actions;
    }
}
}