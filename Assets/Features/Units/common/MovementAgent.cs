using System;
using Features.Pathfinding;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

namespace Features.Units {
public class MovementAgent : MonoBehaviour {
    public float Speed = 3f;
    public float _stoppingDistance = 1f;

    private Vector3[] _path = new Vector3[0];
    private int _currentNode = -1;
    private MapManager _mapManager;
    private Action _cancelPath;

    private void Start() {
        _mapManager = MapManager.Instance;
    }

    private void Update() {
        if (_currentNode < 1) return;
        var nextTarget = _path[_currentNode];
        var position = transform.position;
        var moveDir = Vector3.Normalize(nextTarget - position);
        var newPos = position + moveDir * (Speed * Time.deltaTime);
        transform.position = newPos;
        if (math.distance(position, nextTarget) < _stoppingDistance) {
            _currentNode--;
        }
    }

    public void SetDestination(Vector3 pos) {
        _cancelPath?.Invoke();
        _cancelPath = _mapManager.RequestPath(new PathRequest {
            From = transform.position,
            To = pos,
            Movement = transform,
            Callback = path => {
                _path = path;
                _currentNode = path.Length - 1;
            }
        });
    }
}
}