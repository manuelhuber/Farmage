using System;
using System.Collections.Generic;
using System.Linq;
using Features.Units;
using Grimity.Actions;
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

    private void Update() {
        if (_currentTarget != null && !_currentTarget.Equals(null)) return;
        GetNewTarget();
    }

    private void Shoot() {
        _currentTarget.TakeDamage(damage);
    }

    private bool GetNewTarget() {
        _targets.RemoveAll(destructible => destructible == null);
        if (_targets.Count == 0) return false;
        _currentTarget = _targets.First();
        return true;
    }


    private void OnTriggerEnter(Collider other) {
        var target = other.gameObject.GetComponent<Mortal>();
        if (target != null && target.team == Team.Aliens) {
            _targets.Add(target);
        }
    }

    private void OnTriggerExit(Collider other) {
        var target = other.gameObject.GetComponent<Mortal>();
        _targets.Remove(target);
        if (target == _currentTarget) {
            GetNewTarget();
        }
    }
}
}