using System;
using System.Collections.Generic;
using System.Linq;
using Features.Units;
using Grimity.Actions;
using Ludiq.PeekCore;
using UnityEngine;

namespace Features.Building.Structures.Turret {
public class Turret : MonoBehaviour {
    private SphereCollider _sphereCollider;
    public int range;
    public int damage;
    public float attackSpeed;

    private readonly List<Mortal> _targets = new List<Mortal>();
    private Mortal _currentTarget;
    private IntervaledAction _attack;


    // Start is called before the first frame update
    private void Start() {
        _sphereCollider = gameObject.AddComponent<SphereCollider>();
        _sphereCollider.isTrigger = true;
        _sphereCollider.radius = range;
        _attack = gameObject.AddComponent<IntervaledAction>();
        _attack.action = Shoot;
        _attack.interval = attackSpeed;
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

        if (_currentTarget == null) {
            GetNewTarget();
        }
    }

    private void RemoveTarget(Mortal target) {
        _targets.Remove(target);
        if (_currentTarget == target) {
            GetNewTarget();
        }
    }

    private void OnTriggerExit(Collider other) {
        var target = other.gameObject.GetComponent<Mortal>();
        RemoveTarget(target);
    }
}
}