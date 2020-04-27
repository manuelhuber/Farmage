using System;
using Boo.Lang;
using Grimity.Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Resources {
public class ResourceManager : GrimitySingleton<ResourceManager> {
    private readonly List<Tuple<Cost, Action<bool>>> _callbacks = new List<Tuple<Cost, Action<bool>>>();
    private Cost _have = new Cost {cash = 100};
    private Text _text;

    private void Start() {
        _text = GameObject.FindWithTag("ResourceText").GetComponent<Text>();
        Add(new Cost {cash = 100});
        onChange();
    }

    public Cost Add(Cost change) {
        _have += change;
        onChange();
        return _have;
    }

    public bool Pay(Cost change) {
        if (!CanBePayed(change)) return false;

        _have -= change;
        onChange();
        return true;
    }

    private void onChange() {
        _text.text = _have.ToString();
        foreach (var callback in _callbacks) callback.Item2(CanBePayed(callback.Item1));
    }

    public bool subscribe(Cost cost, Action<bool> callback) {
        _callbacks.Add(new Tuple<Cost, Action<bool>>(cost, callback));
        return CanBePayed(cost);
    }

    public bool CanBePayed(Cost cost) {
        return cost <= _have;
    }
}
}