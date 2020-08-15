using System;
using Features.Health;
using Features.Time;
using Grimity.Actions;
using UnityEngine;

namespace Features.Building.Structures.FieldTower {
public class FieldTower : MonoBehaviour {
    public GameObject spherePrefab;
    public int regenPerSecond;
    public float sphereRebuildDelay;

    private PeriodicalAction _shieldRegeneration;
    public int SphereHp;
    private GameTime _time;

    private void Awake() {
        _time = GameTime.Instance;
        _shieldRegeneration = gameObject.AddComponent<PeriodicalAction>();
        _shieldRegeneration.getTime = () => _time.Time;
        _shieldRegeneration.interval = 1;
    }

    private void Start() {
        RebuildSphere();
    }

    private void RebuildSphere() {
        var sphere = Instantiate(spherePrefab, transform);
        this.After(0.5f)
            .Do(() => {
                var mortal = sphere.AddComponent<Mortal>();
                mortal.MaxHitpoints = SphereHp;
                mortal.TakeDamage(-SphereHp);
                mortal.onDeath.AddListener(() => {
                    _shieldRegeneration.action = null;
                    sphereRebuildDelay = 1f;
                    this.After(sphereRebuildDelay).Do(RebuildSphere);
                });
                _shieldRegeneration.action = () => {
                    mortal.TakeDamage(-regenPerSecond);
                    return true;
                };
            });
    }
}
}