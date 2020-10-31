using System.Collections.Generic;
using System.Linq;
using Features.Save;
using UnityEngine;

namespace Features.Resources {
public class ResourceObject : MonoBehaviour, ISavableComponent<ResourceObjectData> {
    public Resource resource;
    public int count = 1;

    #region Save

    public string SaveKey => "ResourceObject";

    public ResourceObjectData Save() {
        return new ResourceObjectData {Count = count, ResourceKey = resource.name};
    }

    public void Load(ResourceObjectData data, IReadOnlyDictionary<string, GameObject> objects) {
        count = data.Count;
        var res = UnityEngine.Resources.FindObjectsOfTypeAll<Resource>()
            .First(resource1 => resource1.key == data.ResourceKey);
        if (res == null) {
            Debug.LogWarning(
                $"Tried to load storage for resource={data.ResourceKey} but this storage doesn't have this resource anymore");
        }
    }

    #endregion
}

[SerializeField]
public struct ResourceObjectData {
    public int Count;
    public string ResourceKey;
}
}