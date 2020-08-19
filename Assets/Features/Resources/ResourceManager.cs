using Grimity.Data;
using Grimity.Singleton;
using UnityEngine.UI;

namespace Features.Resources {
public class ResourceManager : GrimitySingleton<ResourceManager> {
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
}
}