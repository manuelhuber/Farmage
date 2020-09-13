using System.Collections.Generic;
using Features.Health;
using Features.Save;
using Features.Tasks;
using UnityEngine;

namespace Features.Building.Structures {
public class Building : MonoBehaviour, ISavableComponent<bool> {
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

    #region Save

    public string SaveKey => "building";

    public bool Save() {
        return _waitingForRepair;
    }

    public void Load(bool data, IReadOnlyDictionary<string, GameObject> objects) {
        _waitingForRepair = data;
    }

    #endregion
}
}