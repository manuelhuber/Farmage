using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Features.Common;
using Features.Health;
using Features.Time;
using Grimity.Collections;
using Grimity.ScriptableObject;
using MonKey.Extensions;
using UnityEngine;

namespace Features.Enemies {
public class EnemyManager : Manager<EnemyManager> {
    public EnemyWave wave;

    public RuntimeGameObjectSet allFarmerBuildings;

    public GameObject[] spawnPoints;
    public float waveInterval = 5f;
    public int DeathCount { get; private set; }
    private List<Mortal> _availableBuildings = new List<Mortal>();


    private float _lastSpawn;
    private GameTime _time;

    private void Awake() {
        _time = GameTime.Instance;
        allFarmerBuildings.OnChange += OnBuildingChange;
    }

    private void Update() {
        if (_time.Time - _lastSpawn < waveInterval) return;
        if (_availableBuildings.IsEmpty()) return;
        var target = _availableBuildings.GetRandomElement();
        var spawnLocation = spawnPoints.GetRandomElement().transform;
        foreach (var enemySpawnInfo in wave.spawns) {
            Spawn(enemySpawnInfo, spawnLocation, target);
        }

        _lastSpawn = _time.Time;
    }

    private void OnBuildingChange(ReadOnlyCollection<GameObject> items) {
        _availableBuildings = items.Select(go => go.GetComponent<Mortal>()).ToList();
    }

    private void Spawn(EnemySpawnInfo enemySpawnInfo,
                       Transform spawnLocation,
                       Mortal target
    ) {
        var count = Random.Range(enemySpawnInfo.min, enemySpawnInfo.max);

        for (var i = 0; i < count; i++) {
            var position = spawnLocation.position;
            position += new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
            spawnLocation.position = position;
            var enemy = Instantiate(enemySpawnInfo.prefab,
                position,
                spawnLocation.rotation,
                transform);
            enemy.GetComponent<Mortal>().onDeath.AddListener(() => DeathCount++);
            enemy.GetComponent<EnemyScript>().DefaultTarget = target;
        }
    }
}
}