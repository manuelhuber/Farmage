using UnityEngine;

namespace Features.Save {
public static class SaveExtensions {
    public static string getSaveID(this MonoBehaviour mono) {
        return mono.gameObject.GetInstanceID().ToString();
    }
}
}