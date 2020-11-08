using System;
using System.Collections.Generic;
using System.Linq;
using Features.Common;
using Features.Pathfinding;
using Features.Save;
using Features.Time;
using Unity.Mathematics;
using UnityEngine;

namespace Features.Units.Common {
public class MovementAgent : MonoBehaviour, ISavableComponent<MovementAgentData> {
    public float speed = 3f;
    public float stoppingDistance = 1f;
    public float turnSpeed = 10f;
    public bool IsMoving => !IsStopped && _currentNode > -1;
    public bool HasArrived { get; private set; }
    public bool IsStopped { get; set; }
    public Vector3 CurrentDestination { get; private set; }

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
        if (!IsMoving) return;
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
        _cancelPath = _mapManager.RequestPath(new PathRequest {
            From = transform.position,
            To = pos,
            Movement = transform,
            Callback = path => {
                _path = bruteMove ? path.Prepend(pos).ToArray() : path;
                _currentNode = _path.Length - 1;
                if (_currentNode == -1) HasArrived = true;
            }
        });
    }

    #region Save

    public string SaveKey => "MovementAgent";

    public MovementAgentData Save() {
        var hasDestination = _path.Length > 0;
        return new MovementAgentData {
            destination = hasDestination ? SerialisableVector3.From(_path[0]) : new SerialisableVector3(),
            isStopped = IsStopped,
            hasDestination = hasDestination
        };
    }

    public void Load(MovementAgentData data, IReadOnlyDictionary<string, GameObject> objects) {
        if (data.hasDestination) {
            SetDestination(data.destination.To(),true);
        }

        IsStopped = data.isStopped;
    }

    #endregion
}

[Serializable]
public struct MovementAgentData {
    public SerialisableVector3 destination;
    public bool isStopped;
    public bool hasDestination;
}
}