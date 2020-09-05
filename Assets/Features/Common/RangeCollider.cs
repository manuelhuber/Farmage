using System;
using System.Collections.Generic;
using UnityEngine;

namespace Features.Common {
public class RangeCollider : MonoBehaviour {
    public const string RangeColliderTag = "RangeCollider";
    public const int RangeColliderLayer = 2; // "Ignore Raycast"
    private SphereCollider _sphereCollider;

    public float Range {
        get => _range;
        set {
            _range = value;
            _sphereCollider.radius = value;
        }
    }

    private readonly HashSet<Action<Collider>> _onEnter = new HashSet<Action<Collider>>();
    private readonly HashSet<Action<Collider>> _onExit = new HashSet<Action<Collider>>();
    private float _range;

    private void Awake() {
        var go = gameObject;
        _sphereCollider = go.AddComponent<SphereCollider>();
        _sphereCollider.isTrigger = true;
        go.tag = RangeColliderTag;
        go.layer = RangeColliderLayer;
    }

    public void OnEnter(Action<Collider> callback) {
        _onEnter.Add(callback);
    }

    public void OnExit(Action<Collider> callback) {
        _onExit.Add(callback);
    }

    private void OnTriggerEnter(Collider other) {
        foreach (var action in _onEnter) {
            action(other);
        }
    }

    private void OnTriggerExit(Collider other) {
        foreach (var action in _onExit) {
            action(other);
        }
    }
}
}