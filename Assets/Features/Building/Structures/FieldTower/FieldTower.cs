using System;
using Features.Health;
using Grimity.Actions;
using UnityEngine;

namespace Features.Building.Structures.FieldTower {
public class FieldTower : MonoBehaviour {
    public GameObject spherePrefab;
    public int regenPerSecond;
    public float sphereRebuildDelay;

    private PeriodicalAction _shieldRegeneration;
    public int SphereHp;

    private void Awake() {
        _shieldRegeneration = gameObject.AddComponent<PeriodicalAction>();
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