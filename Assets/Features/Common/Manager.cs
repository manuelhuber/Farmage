using UnityEngine;

namespace Features.Common {
public class Manager<T> : MonoBehaviour where T : MonoBehaviour {
    public static T Instance => FindObjectOfType<T>();
}
}