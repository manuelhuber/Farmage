using System;
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
    public Grimity.Data.IObservable<Cost> Have => _have;

    private void Start() {
        Add(new Cost {cash = 1000});
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

    public Optional<Tuple<Storable, Storage>> FindItem(ItemType type) {
        var storage = GetStorages()
            .First(s => s.type == type);
        var item = storage.ReserveItem(storable => storable.IsType(type));
        return !item.HasValue
            ? Optional<Tuple<Storable, Storage>>.NoValue()
            : Optional<Tuple<Storable, Storage>>.Of(new Tuple<Storable, Storage>(item.Value, storage));
    }

    public GameObject GetBestStorage(Storable newLoot) {
        return GetStorages()
            .Where(storage => !storage.IsFull)
            .FirstOrDefault(storage => newLoot.IsType(storage.type))
            ?.gameObject;
    }

    private IEnumerable<Storage> GetStorages() {
        return allFarmerBuildings.Items.Select(o => o.GetComponent<Storage>())
            .Where(storage => storage != null);
    }

    #region Save

    public string SaveKey => "resources";

    public string Save() {
        return _have.Value.ToJson();
    }

    public void Load(string rawData, IReadOnlyDictionary<string, GameObject> objects) {
        _have.Set(rawData.FromJson<Cost>());
    }

    #endregion
}
}