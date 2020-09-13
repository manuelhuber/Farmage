using System.Collections.Generic;
using UnityEngine;

namespace Features.Save {
public interface ISavableComponent<T> where T : struct {
    string SaveKey { get; }
    T Save();
    void Load(T data, IReadOnlyDictionary<string, GameObject> objects);
}
}