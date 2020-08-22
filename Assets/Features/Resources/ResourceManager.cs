using System.Collections.Generic;
using Features.Save;
using Grimity.Data;
using Grimity.Singleton;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Resources {
public class ResourceManager : GrimitySingleton<ResourceManager>, ISavableComponent {
    private Text _text;
    private Observable<Cost> _have = new Observable<Cost>(new Cost());
    public IObservable<Cost> Have => _have;

    private void Start() {
        Add(new Cost {cash = 100});
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

    public string SaveKey => "resources";

    public string Save() {
        return _have.Value.ToJson();
    }

    public void Load(string rawData, IReadOnlyDictionary<string, GameObject> objects) {
        _have.Set(rawData.FromJson<Cost>());
    }
}
}