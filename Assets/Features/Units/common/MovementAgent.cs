using System;
using System.Linq;
using Features.Pathfinding;
using Unity.Mathematics;
using UnityEngine;

namespace Features.Units.Common {
public class MovementAgent : MonoBehaviour {
    public float speed = 3f;
    public float stoppingDistance = 1f;
    public float turnSpeed = 10f;

    public bool HasArrived {
        get => _hasArrived;
        set => _hasArrived = value;
    }

    public bool isStopped {
        get => _isStopped;
        set => _isStopped = value;
    }

    private Vector3[] _path = new Vector3[0];
    private int _currentNode = -1;
    private MapManager _mapManager;
    private Action _cancelPath;
    private bool _isStopped;
    [SerializeField] private bool _hasArrived;

    private void OnDrawGizmos() {
        var prev = transform.position;
        foreach (var pathPos in _path) {
            Gizmos.DrawLine(prev, pathPos);
            Gizmos.DrawSphere(pathPos, 1);
            prev = pathPos;
        }
    }

    private void Awake() {
        _mapManager = MapManager.Instance;
    }

    private void Update() {
        if (_isStopped || _currentNode < 0) return;
        var nextTarget = _path[_currentNode];
        var trans = transform;
        var position = trans.position;
        nextTarget.y = position.y;
        var neededRotation = Quaternion.LookRotation((nextTarget - position));
        trans.rotation =
            Quaternion.RotateTowards(trans.rotation, neededRotation, Time.deltaTime * turnSpeed * 100);
        // var moveDir = Vector3.Normalize(nextTarget - position);
        var newPos = position + trans.forward * (speed * Time.deltaTime);
        trans.position = newPos;
        if (!(math.distance(position, nextTarget) < stoppingDistance)) return;
        _currentNode--;
        if (_currentNode == -1) HasArrived = true;
    }

    public void SetDestination(Vector3 pos, bool bruteMove = false) {
        HasArrived = false;
        _cancelPath?.Invoke();
        _cancelPath = _mapManager.RequestPath(new PathRequest {
            From = transform.position,
            To = pos,
            Movement = transform,
            Callback = path => {
                var legitPath = path;
                _path = bruteMove ? legitPath.Prepend(pos).ToArray() : legitPath;
                _currentNode = _path.Length - 1;
                if (_currentNode == -1) _hasArrived = true;
            }
        });
    }
}
}