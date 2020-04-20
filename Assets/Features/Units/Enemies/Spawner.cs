using System;
using Features.Enemies;
using Grimity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour {
    public float waveInterval = 5f;
    public GameObject[] spawnPoints;
    public GameObject enemyPrefab;
    public RangeInt spawnCount = new RangeInt(5, 10);

    private float _lastSpawn;
    private GameObject[] _hqs;

    private void Start() { }

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