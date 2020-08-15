using System;
using System.Collections.Generic;
using System.Linq;
using Features.Health;
using Features.Time;
using Features.Units.Common;
using Grimity.Actions;
using UnityEngine;

namespace Features.Building.Structures.Turret {
public class Turret : MonoBehaviour {
    private readonly List<Mortal> _targets = new List<Mortal>();
    private PeriodicalAction _attack;
    private Mortal _currentTarget;
    private SphereCollider _sphereCollider;
    public float attackSpeed;
    public int damage;
    public int range;
    private GameTime _time;


    private void Awake() {
        _time = GameTime.Instance;
        _sphereCollider = gameObject.AddComponent<SphereCollider>();
        _sphereCollider.isTrigger = true;
        _sphereCollider.radius = range;
        _attack = gameObject.AddComponent<PeriodicalAction>();
        _attack.action = Shoot;
        _attack.interval = attackSpeed;
        _attack.getTime = () => _time.Time;
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

        _currentTarget = _targets.First();
        _attack.IsRunning = true;
    }


    private void OnTriggerEnter(Collider other) {
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

    private void OnTriggerExit(Collider other) {
        var target = other.gameObject.GetComponent<Mortal>();
        RemoveTarget(target);
    }
}
}