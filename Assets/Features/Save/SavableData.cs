using System;
using System.Collections.Generic;
using System.Linq;
using Ludiq.PeekCore.TinyJson;
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
            // Cant get a ISavableComponentTyped<T> reference here since we don't know type T at compile time
            object componentData = ((dynamic) savable).Save();
            var key = (string) ((dynamic) savable).SaveKey;
            saveData[key] = componentData.ToJson();
        }

        return saveData;
    }

    public void LoadComponents(IReadOnlyDictionary<string, string> data,
                               IReadOnlyDictionary<string, GameObject> objects) {
        var savables = GetSavableComponents();
        foreach (var component in savables) {
            var genericType = GetSavableGenericType(component);

            var key = (string) ((dynamic) component).SaveKey;
            if (!data.TryGetValue(key, out var componentDataJson)) continue;
            dynamic componentData = typeof(JsonParser)
                .GetMethod("FromJson")
                ?.MakeGenericMethod(genericType)
                .Invoke(componentDataJson, new object[] {componentDataJson});

            component.GetType()
                .GetMethod("Load")
                ?.Invoke(component, new[] {componentData, objects});
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

    /// <summary>
    ///     Returns the type of the generic of the components ISavableComponentTyped
    /// </summary>
    /// <param name="monoBehaviour"></param>
    /// <returns></returns>
    private static Type GetSavableGenericType(MonoBehaviour monoBehaviour) {
        return monoBehaviour.GetType()
            .GetInterfaces()
            .First(x =>
                x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISavableComponent<>))
            .GetGenericArguments()[0];
    }
}
}