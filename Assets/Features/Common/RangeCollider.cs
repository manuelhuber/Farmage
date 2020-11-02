using System;
using System.Collections.Generic;
using Constants;
using UnityEngine;

namespace Features.Common {
public class RangeCollider : MonoBehaviour {
    public const int RangeColliderLayer = 2; // "Ignore Raycast"

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
    private SphereCollider _sphereCollider;

    private void Awake() {
        var go = gameObject;
        _sphereCollider = go.AddComponent<SphereCollider>();
        _sphereCollider.isTrigger = true;
        go.tag = Tags.RangeColliderTag;
        go.layer = RangeColliderLayer;
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


    public static RangeCollider AddTo(GameObject parent, float range) {
        var rangeObject = new GameObject("Range");
        rangeObject.transform.SetParent(parent.transform);
        rangeObject.transform.localPosition = Vector3.zero;
        var rangeCollider = rangeObject.AddComponent<RangeCollider>();
        rangeCollider.Range = range;
        return rangeCollider;
    }

    public void OnEnter(Action<Collider> callback) {
        _onEnter.Add(callback);
    }

    public void OnExit(Action<Collider> callback) {
        _onExit.Add(callback);
    }
}
}