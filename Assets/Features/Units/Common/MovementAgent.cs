using System;
using System.Collections.Generic;
using System.Linq;
using Features.Common;
using Features.Pathfinding;
using Features.Time;
using Grimity.Data;
using Unity.Mathematics;
using UnityEngine;

namespace Features.Units.Common {
public class MovementAgent : MonoBehaviour {
    public float speed = 3f;
    public float stoppingDistance = 1f;
    public float turnSpeed = 10f;
    public Grimity.Data.IObservable<bool> IsMoving => _isMovingObservable;
    public bool HasArrived { get; private set; }
    public bool IsStopped { get; set; }
    public Vector3 CurrentDestination { get; private set; }
    private readonly Observable<bool> _isMovingObservable = new Observable<bool>(false);

    private Action _cancelPath;
    private int _currentNode = -1;
    private MapManager _mapManager;
    private Vector3[] _path = new Vector3[0];
    private Rigidbody _rigidbody;
    private GameTime _time;

    private void Awake() {
        _mapManager = MapManager.Instance;
        _time = GameTime.Instance;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        var isMoving = !IsStopped && _currentNode > -1;
        _isMovingObservable.Set(isMoving);
        if (!isMoving) return;
        var nextTarget = _path[_currentNode];
        var trans = transform;
        var position = trans.position;
        nextTarget.y = position.y;
        var neededRotation = Quaternion.LookRotation(nextTarget - position);
        trans.rotation =
            Quaternion.RotateTowards(trans.rotation, neededRotation, _time.FixedDeltaTime * turnSpeed * 100);
        var newPos = position + trans.forward * (speed * _time.FixedDeltaTime);
        _rigidbody.MovePosition(newPos);
        if (!(math.distance(position, nextTarget) < stoppingDistance)) return;
        _currentNode--;
        if (_currentNode == -1) HasArrived = true;
    }

    private void OnDrawGizmos() {
        var prev = transform.position;
        foreach (var pathPos in _path) {
            Gizmos.DrawLine(prev, pathPos);
            Gizmos.DrawSphere(pathPos, 1);
            prev = pathPos;
        }
    }

    public void AbandonDestination() {
        _currentNode = -1;
    }

    public void SetDestination(Vector3 pos, bool bruteMove) {
        CurrentDestination = pos;
        HasArrived = false;
        _cancelPath?.Invoke();
        var trans = transform;
        _cancelPath = _mapManager.RequestPath(new PathRequest {
            From = trans.position,
            To = pos,
            Movement = trans,
            Callback = path => {
                _path = bruteMove ? path.Prepend(pos).ToArray() : path;
                _currentNode = _path.Length - 1;
                if (_currentNode == -1) HasArrived = true;
            }
        });
    }
}
}