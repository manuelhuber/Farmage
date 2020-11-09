using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Features.Health;
using Features.Time;
using Grimity.Collections;
using Grimity.ScriptableObject;
using MonKey.Extensions;
using UnityEngine;

namespace Features.Units.Enemies {
public class Spawner : MonoBehaviour {
    public RuntimeGameObjectSet allFarmerBuildings;
    public GameObject enemyPrefab;

    public GameObject[] spawnPoints;
    public float waveInterval = 5f;

    public int minSpawnCount;
    public int maxSpawnCount;

    private float _lastSpawn;
    private GameTime _time;
    private List<Mortal> _availableBuildings = new List<Mortal>();

    private void Awake() {
        _time = GameTime.Instance;
        allFarmerBuildings.OnChange += OnBuildingChange;
    }

    private void OnBuildingChange(ReadOnlyCollection<GameObject> items) {
        _availableBuildings = items.Select(go => go.GetComponent<Mortal>()).ToList();
    }

    private void Update() {
        if (_time.Time - _lastSpawn < waveInterval) return;
        if (_availableBuildings.IsEmpty()) return;
        var target = _availableBuildings.GetRandomElement();
        var spawnLocation = spawnPoints.GetRandomElement().transform;
        var count = Random.Range(minSpawnCount, maxSpawnCount);
        var offset = 0;
        for (var i = 0; i < count; i++) {
            var offsetLocation = spawnLocation.position + new Vector3(0, 0, offset);
            var enemy = Instantiate(enemyPrefab, offsetLocation, spawnLocation.rotation, transform);
            enemy.GetComponent<EnemyScript>().DefaultTarget = target;
            offset += 10;
        }

        _lastSpawn = _time.Time;
    }
}
}