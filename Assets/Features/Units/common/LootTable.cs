using System;
using UnityEngine;

namespace Features.Units.common {
[Serializable]
public struct Loot {
    public GameObject prefab;
    public float dropchance;
}

[CreateAssetMenu(menuName = "loot/loot table")]
public class LootTable : ScriptableObject {
    public Loot[] loot;
}
}