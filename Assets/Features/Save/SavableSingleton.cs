using System.Collections.Generic;
using UnityEngine;

namespace Features.Save {
public class SavableSingleton : SavableData {
    [SerializeField] public string Key;

    public override Dictionary<string, string> Save() {
        return GetSaveDataFromComponents();
    }

    public override void Load(IReadOnlyDictionary<string, string> data,
                              IReadOnlyDictionary<string, GameObject> objects) {
        LoadComponents(data, objects);
    }
}
}