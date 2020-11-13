using System;
using System.Collections.Generic;
using System.Linq;
using Features.Common;
using Features.Health;
using Features.Save;
using Features.Time;
using Grimity.Actions;
using UnityEngine;

namespace Features.Building.Structures.Turret {
public class Turret : MonoBehaviour, ISavableComponent<TurretData> {
    public float attackSpeed;
    public int damage;
    public int range;
    public GameObject turret;
    private readonly List<Mortal> _targets = new List<Mortal>();
    private PeriodicalAction _attack;
    private Mortal _currentTarget;
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

    private void Update() {
        if (_currentTarget != null) {
            var turretTransform = turret.transform;
            turretTransform.LookAt(_currentTarget.transform);
            turretTransform.eulerAngles =
                new Vector3(0, turretTransform.eulerAngles.y, 0); // lock x and z axis to zero
        }
    }

    private void CreateRangeCollider() {
        var rangeCollider = RangeCollider.AddTo(gameObject, range);
        rangeCollider.OnEnter(OnEnterRange);
        rangeCollider.OnExit(OnLeaveRange);
    }

    private bool Shoot() {
        _currentTarget.TakeDamage(new Damage {Source = gameObject, Amount = damage});
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
        if (target == null || target.Team != Team.Aliens) return;

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

    #region Save

    public string SaveKey => "Turret";

    public TurretData Save() {
        return new TurretData {
            currentTarget = _currentTarget.getSaveID(),
            nextAttack = _attack.NextExecution
        };
    }

    public void Load(TurretData data, IReadOnlyDictionary<string, GameObject> objects) {
        _attack.SetNextExecution(data.nextAttack);
        var target = objects.getBySaveID(data.currentTarget)?.GetComponent<Mortal>();
        if (target != null) {
            SetTarget(target);
        }
    }

    #endregion
}

[Serializable]
public struct TurretData {
    public string currentTarget;
    public float nextAttack;
}
}