using System;
using System.Collections.Generic;
using UnityEngine;

namespace Features.Save {
public abstract class SavableData : MonoBehaviour {
    public abstract Dictionary<string, string> Save();

    public abstract void Load(IReadOnlyDictionary<string, string> data,
                              IReadOnlyDictionary<string, GameObject> objects);

    protected Dictionary<string, string> GetSaveDataFromComponents() {
        var saveData = new Dictionary<string, string>();
        var components = GetComponents<MonoBehaviour>();
        foreach (var component in components) {
            if (component is ISavableComponent savableComponent) {
                saveData[savableComponent.SaveKey] = savableComponent.Save();
            }
        }

        return saveData;
    }

    public void LoadComponents(IReadOnlyDictionary<string, string> data,
                               IReadOnlyDictionary<string, GameObject> objects) {
        var components = GetComponents<MonoBehaviour>();

        foreach (var component in components) {
            if (!(component is ISavableComponent savableComponent)) continue;
            if (data.TryGetValue(savableComponent.SaveKey, out var componentData)) {
                savableComponent.Load(componentData, objects);
            }
        }
    }
}
}