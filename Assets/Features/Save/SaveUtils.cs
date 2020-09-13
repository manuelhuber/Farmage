using System;
using System.Collections.Generic;
using System.Linq;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;

namespace Features.Save {
public static class SaveUtils {
    /// <summary>
    ///     Returns the type of the generic of the components ISavableComponent
    /// </summary>
    /// <param name="something"></param>
    /// <returns></returns>
    public static Type GetSavableGenericType(object something) {
        return something.GetType()
            .GetInterfaces()
            .First(x =>
                x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISavableComponent<>))
            ?.GetGenericArguments()[0];
    }

    /// <summary>
    ///     ONLY CALL THIS ON OBJECTS THAT IMPLEMENT ISavableComponent
    /// </summary>
    /// <param name="component">something that's ISavableComponent</param>
    /// <param name="objects">a dictionary of existing objects mapped by ID</param>
    /// <param name="componentDataJson">json data to load into the component</param>
    public static void LoadISavable(this object component,
                                    IReadOnlyDictionary<string, GameObject> objects,
                                    string componentDataJson) {
        var genericType = GetSavableGenericType(component);
        dynamic componentData = typeof(JsonParser)
            .GetMethod("FromJson")
            ?.MakeGenericMethod(genericType)
            .Invoke(componentDataJson, new object[] {componentDataJson});

        component.GetType()
            .GetMethod("Load")
            ?.Invoke(component, new[] {componentData, objects});
    }

    /// <summary>
    ///     ONLY CALL THIS ON OBJECTS THAT IMPLEMENT ISavableComponent
    /// </summary>
    /// <param name="savable"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string SerialiseISavable(this object savable, out string key) {
        // Cant get a ISavableComponentTyped<T> reference here since we don't know type T at compile time
        object componentData = ((dynamic) savable).Save();
        key = (string) ((dynamic) savable).SaveKey;
        return componentData.ToJson();
    }
}
}