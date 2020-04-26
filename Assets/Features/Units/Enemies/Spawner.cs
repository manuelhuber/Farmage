using Features.Enemies;
using Grimity.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour {
    private GameObject[] _hqs;

    private float _lastSpawn;
    public GameObject enemyPrefab;
    public RangeInt spawnCount = new RangeInt(1, 1);
    public GameObject[] spawnPoints;
    public float waveInterval = 5f;

    private void Start() {
    }

    private void Update() {
        if (Time.time - _lastSpawn < waveInterval) return;
        _hqs = GameObject.FindGameObjectsWithTag("HQ");
        _lastSpawn = Time.time;
        var count = Random.Range(spawnCount.start, spawnCount.end);
        for (var i = 0; i < count; i++) {
            var spawnLocation = spawnPoints.GetRandomElement().transform;
            var enemy = Instantiate(enemyPrefab, spawnLocation.position, spawnLocation.rotation, transform);
            enemy.GetComponent<EnemyScript>().setTargets(_hqs);
        }
    }
}