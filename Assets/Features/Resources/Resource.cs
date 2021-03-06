using UnityEngine;

namespace Features.Resources {
[CreateAssetMenu(fileName = "Resource", menuName = "new Resource", order = 0)]
public class Resource : ScriptableObject {
    public Cost worth;
    public GameObject prefab;

    public ResourceObject CreateResourceObject(int count) {
        var o = Instantiate(prefab);
        var resourceObject = o.AddComponent<ResourceObject>();
        resourceObject.resource = this;
        resourceObject.count = count;
        return resourceObject;
    }
}
}