using System;
using System.Collections.Generic;
using Constants;
using Features.Health;
using Features.Save;
using Features.Time;
using Grimity.Actions;
using UnityEngine;

namespace Features.Building.Structures.FieldTower {
public class FieldTower : MonoBehaviour, ISavableComponent<FieldTowerData> {
    public GameObject spherePrefab;
    public int regenPerSecond;
    public float sphereRebuildDelay = 1f;
    public int sphereHp;
    private DelayedAction _rebuildAction;

    private PeriodicalAction _shieldRegeneration;
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
        _rebuildAction = this.Do(() => BuildSphere(sphereHp)).withTime(_time.getTime);
        _rebuildAction.After(sphereRebuildDelay);
    }

    private void BuildSphere(int initialHp) {
        _rebuildAction = null;
        var sphere = Instantiate(spherePrefab, transform).AddComponent<Mortal>();
        sphere.tag = Tags.SphereTag;
        sphere.MaxHitpoints = sphereHp;
        sphere.TakeDamage(-initialHp);
        sphere.onDeath.AddListener(() => {
            _shieldRegeneration.action = null;
            RebuildSphere();
        });
        _shieldRegeneration.action = () => {
            sphere.TakeDamage(-regenPerSecond);
            return true;
        };
    }

    #region Save

    public string SaveKey => "FieldTower";

    public FieldTowerData Save() {
        return new FieldTowerData {
            currentShieldHp = 1,
            shieldRebuildTimestamp = _rebuildAction != null ? _rebuildAction.TargetTime : -1
        };
    }

    public void Load(FieldTowerData data, IReadOnlyDictionary<string, GameObject> objects) {
        if (data.shieldRebuildTimestamp >= 0) {
            this.Do(() => BuildSphere(sphereHp)).withTime(_time.getTime).At(data.shieldRebuildTimestamp);
        } else {
            BuildSphere(data.currentShieldHp);
        }
    }

    #endregion
}

[Serializable]
public struct FieldTowerData {
    public int currentShieldHp;
    public float shieldRebuildTimestamp;
}
}