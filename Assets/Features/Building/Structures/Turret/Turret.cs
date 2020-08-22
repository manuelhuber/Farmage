using System;
using System.Collections.Generic;
using System.Linq;
using Features.Common;
using Features.Health;
using Features.Save;
using Features.Time;
using Grimity.Actions;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;

namespace Features.Building.Structures.Turret {
public class Turret : MonoBehaviour, ISavableComponent {
    private readonly List<Mortal> _targets = new List<Mortal>();
    private PeriodicalAction _attack;
    private Mortal _currentTarget;
    public float attackSpeed;
    public int damage;
    public int range;
    private GameTime _time;


    private void Awake() {
        _time = GameTime.Instance;
        _attack = gameObject.AddComponent<PeriodicalAction>();
        _attack.initialDelay = true;
        _attack.action = Shoot;
        _attack.interval = attackSpeed;
        _attack.getTime = () => _time.Time;
        CreateRangeCollider();
    }

    private void CreateRangeCollider() {
        var rangeObject = new GameObject("Range");
        rangeObject.transform.SetParent(transform);
        rangeObject.transform.localPosition = Vector3.zero;
        var rangeCollider = rangeObject.AddComponent<RangeCollider>();
        rangeCollider.Range = range;
        rangeCollider.OnEnter(OnEnterRange);
        rangeCollider.OnExit(OnLeaveRange);
    }

    private bool Shoot() {
        _currentTarget.TakeDamage(damage);
        return true;
    }

    private void GetNewTarget() {
        if (_targets.Count == 0) {
            _attack.IsRunning = false;
            return;
        }

        SetTarget(_targets.First());
    }

    private void SetTarget(Mortal newTarget) {
        _currentTarget = newTarget;
        _attack.IsRunning = true;
    }


    private void OnEnterRange(Collider other) {
        var target = other.gameObject.GetComponent<Mortal>();
        if (target == null || target.team != Team.Aliens) return;

        target.onDeath.AddListener(() => { RemoveTarget(target); });

        _targets.Add(target);

        if (_currentTarget == null) GetNewTarget();
    }

    private void RemoveTarget(Mortal target) {
        _targets.Remove(target);
        if (_currentTarget == target) GetNewTarget();
    }

    private void OnLeaveRange(Collider other) {
        var target = other.gameObject.GetComponent<Mortal>();
        RemoveTarget(target);
    }

    public string SaveKey => "Turret";

    public string Save() {
        return new TurretData {
            currentTarget = _currentTarget.getSaveID(),
            nextAttack = _attack.NextExecution
        }.ToJson();
    }

    public void Load(string rawData, IReadOnlyDictionary<string, GameObject> objects) {
        var turretData = rawData.FromJson<TurretData>();
        _attack.SetNextExecution(turretData.nextAttack);
        var target = objects.getBySaveID(turretData.currentTarget)?.GetComponent<Mortal>();
        if (target != null) {
            SetTarget(target);
        }
    }
}

[Serializable]
internal struct TurretData {
    public string currentTarget;
    public float nextAttack;
}
}