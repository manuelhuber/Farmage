using System.Collections.Generic;
using System.Linq;
using Features.Building.Structures.Warehouse;
using Features.Items;
using Features.Save;
using Grimity.Data;
using Grimity.ScriptableObject;
using Grimity.Singleton;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Resources {
public class ResourceManager : GrimitySingleton<ResourceManager>, ISavableComponent {
    public RuntimeGameObjectSet allFarmerBuildings;
    private readonly Observable<Cost> _have = new Observable<Cost>(new Cost());

    private Text _text;
    public IObservable<Cost> Have => _have;

    private void Start() {
        Add(new Cost {cash = 1000});
    }

    public string SaveKey => "resources";

    public string Save() {
        return _have.Value.ToJson();
    }

    public void Load(string rawData, IReadOnlyDictionary<string, GameObject> objects) {
        _have.Set(rawData.FromJson<Cost>());
    }

    public Cost Add(Cost change) {
        _have.Set(_have.Value + change);
        return _have.Value;
    }

    public bool Pay(Cost change) {
        if (!CanBePayed(change)) return false;

        _have.Set(_have.Value - change);
        return true;
    }

    public bool CanBePayed(Cost cost) {
        return cost <= _have.Value;
    }

    public GameObject GetBestStorage(Storable newLoot) {
        return allFarmerBuildings.Items.Select(o => o.GetComponent<Storage>())
            .Where(storage => storage != null)
            .Where(storage => !storage.IsFull)
            .FirstOrDefault(storage => newLoot.IsType(storage.type))
            ?.gameObject;
    }
}
}