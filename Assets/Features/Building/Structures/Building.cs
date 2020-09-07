using System.Collections.Generic;
using Features.Health;
using Features.Save;
using Features.Tasks;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;

namespace Features.Building.Structures {
public class Building : MonoBehaviour, ISavableComponent {
    private readonly bool autoRepair = false;
    private bool _waitingForRepair;

    private void Start() {
        var mortal = GetComponent<Mortal>();
        mortal.Hitpoints.OnChange(hitpoints => {
            if (!autoRepair) return;
            if (hitpoints == mortal.MaxHitpoints) {
                _waitingForRepair = false;
                return;
            }

            if (_waitingForRepair) return;
            TaskManager.Instance.Enqueue(new SimpleTask {type = TaskType.Repair, Payload = gameObject});
            _waitingForRepair = true;
        });
    }

    public string SaveKey => "building";

    public string Save() {
        return _waitingForRepair.ToJson();
    }

    public void Load(string rawData, IReadOnlyDictionary<string, GameObject> objects) {
        _waitingForRepair = rawData.FromJson<bool>();
    }
}
}