using System;
using UnityEngine;

namespace Features.Enemies {
[Serializable]
public class EnemyWave {
    public EnemySpawnInfo[] spawns;
}

[Serializable]
public class EnemySpawnInfo {
    public GameObject prefab;
    public int min;
    public int max;
}
}