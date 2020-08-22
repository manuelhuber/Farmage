using System.Collections.Generic;
using UnityEngine;

namespace Features.Save {
public interface ISavableComponent {
    string SaveKey { get; }
    string Save();
    void Load(string rawData, IReadOnlyDictionary<string, GameObject> objects);
}
}