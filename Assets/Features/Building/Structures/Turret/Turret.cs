using System;
using System.Collections.Generic;
using System.Linq;
using Features.Units;
using UnityEngine;

namespace Features.Building.Structures.Turret {
public class Turret : MonoBehaviour {
    private SphereCollider _sphereCollider;
    public int range;
    public int damage;
    public float attackSpeed;


    private readonly List<Mortal> _targets = new List<Mortal>();
    private Mortal currentTarget;
    private float nextShot;


    // Start is called before the first frame update
    private void Start() {
        _sphereCollider = gameObject.AddComponent<SphereCollider>();
        _sphereCollider.isTrigger = true;
        _sphereCollider.radius = range;
    }

    private void Update() {
        if (currentTarget == null || currentTarget.Equals(null)) {
            if (!GetNewTarget()) return;
        }

        if (Time.time > nextShot) {
            Shoot();
        }
    }

    private void Shoot() {
        currentTarget.DealDamage(damage);
        nextShot = Time.time + attackSpeed;
    }

    private bool GetNewTarget() {
        _targets.RemoveAll(destructible => destructible == null);
        if (_targets.Count == 0) return false;
        currentTarget = _targets.First();
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
        if (target == currentTarget) {
            GetNewTarget();
        }
    }
}
}