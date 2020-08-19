using System.Collections.Generic;
using UnityEngine;

namespace Features.Save {
public interface ISavableComponent {
    string SaveKey { get; }
    string Save();
    void Load(string data, IReadOnlyDictionary<string, GameObject> objects);
}
}