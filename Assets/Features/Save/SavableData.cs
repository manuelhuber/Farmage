using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Features.Save {
/// <summary>
///     The base class for savable stuff
///     There's some reflection magic and dynamic hackyness here to move serialisation out of actual components
/// </summary>
public abstract class SavableData : MonoBehaviour {
    public abstract Dictionary<string, string> Save();

    public abstract void Load(IReadOnlyDictionary<string, string> data,
                              IReadOnlyDictionary<string, GameObject> objects);

    protected Dictionary<string, string> GetSaveDataFromComponents() {
        var saveData = new Dictionary<string, string>();
        var savables = GetSavableComponents();
        foreach (var savable in savables) {
            var json = savable.SerialiseISavable(out var key);
            saveData[key] = json;
        }

        return saveData;
    }

    public void LoadComponents(IReadOnlyDictionary<string, string> data,
                               IReadOnlyDictionary<string, GameObject> objects) {
        var savables = GetSavableComponents();
        foreach (var component in savables) {
            var key = (string) ((dynamic) component).SaveKey;
            if (!data.TryGetValue(key, out var componentDataJson)) continue;
            component.LoadISavable(objects, componentDataJson);
        }
    }

    /// <summary>
    ///     Returns all MonoBehaviours that implement
    /// </summary>
    /// <returns></returns>
    private IEnumerable<MonoBehaviour> GetSavableComponents() {
        var savableType = typeof(ISavableComponent<>);

        bool IsSavable(MonoBehaviour monoBehaviour) {
            return monoBehaviour.GetType()
                .GetInterfaces()
                .Any(x => x.IsGenericType &&
                          x.GetGenericTypeDefinition() == savableType);
        }

        return GetComponents<MonoBehaviour>().Where(IsSavable);
    }
}
}