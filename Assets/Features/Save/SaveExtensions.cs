using System.Collections.Generic;
using UnityEngine;

namespace Features.Save {
public static class SaveExtensions {
    public static string getSaveID(this MonoBehaviour mono) {
        return mono == null ? "" : mono.gameObject.getSaveID();
    }

    public static string getSaveID(this GameObject go) {
        return go == null ? "" : go.GetInstanceID().ToString();
    }

    public static GameObject getBySaveID(this IReadOnlyDictionary<string, GameObject> dictionary,
                                         string saveId) {
        if (saveId == "") return null;
        if (!dictionary.ContainsKey(saveId)) {
            Debug.LogError("Trying to load an object that wasn't saved! FIX ME!");
            return null;
        }

        return dictionary[saveId];
    }
}
}