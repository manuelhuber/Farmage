using System.Linq;
using Features.Health;
using Grimity.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.Units.Enemies {
public class Spawner : MonoBehaviour {
    private GameObject[] _hqs;

    private float _lastSpawn;
    public GameObject enemyPrefab;

    [InfoBox("Min/Max number of enemies spawned per wave")]
    public RangeInt spawnCount = new RangeInt(1, 1);

    public GameObject[] spawnPoints;
    public float waveInterval = 5f;

    private void Update() {
        if (Time.time - _lastSpawn < waveInterval) return;
        _hqs = GameObject.FindGameObjectsWithTag("HQ");
        _lastSpawn = Time.time;
        var count = Random.Range(spawnCount.start, spawnCount.end);
        var offset = 0;
        for (var i = 0; i < count; i++) {
            var spawnLocation = spawnPoints.GetRandomElement().transform;
            var offsetLocation = spawnLocation.position + new Vector3(0, 0, offset);
            var enemy = Instantiate(enemyPrefab, offsetLocation, spawnLocation.rotation, transform);
            enemy.GetComponent<EnemyScript>()
                .SetTargets(_hqs.Select(go => go.GetComponent<Mortal>()).ToList());
            offset += 10;
        }
    }
}
}