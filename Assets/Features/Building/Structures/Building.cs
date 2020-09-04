using System.Collections.Generic;
using Features.Health;
using Features.Queue;
using Features.Save;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;

namespace Features.Building.Structures {
public class Building : MonoBehaviour, ISavableComponent {
    [SerializeField] private JobMultiQueue queue;
    private bool _waitingForRepair;

    private void Start() {
        var mortal = GetComponent<Mortal>();
        mortal.Hitpoints.OnChange(hitpoints => {
            if (hitpoints == mortal.MaxHitpoints) {
                _waitingForRepair = false;
                return;
            }

            if (_waitingForRepair) return;
            queue.Enqueue(new SimpleTask {type = TaskType.Repair, Payload = gameObject});
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