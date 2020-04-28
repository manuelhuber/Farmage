using System;
using UnityEngine;

namespace Features.Items.Loot {
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