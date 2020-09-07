using System;
using System.Collections.Generic;
using System.Linq;
using Features.Common;
using Features.Pathfinding;
using Features.Save;
using Features.Time;
using Ludiq.PeekCore.TinyJson;
using Unity.Mathematics;
using UnityEngine;

namespace Features.Units.Common {
public class MovementAgent : MonoBehaviour, ISavableComponent {
    public float speed = 3f;
    public float stoppingDistance = 1f;
    public float turnSpeed = 10f;
    private Action _cancelPath;
    private int _currentNode = -1;
    private MapManager _mapManager;

    private Vector3[] _path = new Vector3[0];
    private Rigidbody _rigidbody;
    private GameTime _time;

    public bool HasArrived { get; private set; }
    public bool IsStopped { get; set; }

    private void Awake() {
        _mapManager = MapManager.Instance;
        _time = GameTime.Instance;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        if (IsStopped || _currentNode < 0) return;
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

    public string SaveKey => "MovementAgent";

    public string Save() {
        if (_path.Length < 1) {
            return "";
        }

        var movementData = new MovementData {destination = SerialisableVector3.From(_path[0])};
        return movementData.ToJson();
    }

    public void Load(string rawData, IReadOnlyDictionary<string, GameObject> objects) {
        if (rawData.Length < 1) return;
        var movementData = rawData.FromJson<MovementData>();
        SetDestination(movementData.destination.To());
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
                if (_currentNode == -1) HasArrived = true;
            }
        });
    }
}

[Serializable]
internal struct MovementData {
    public SerialisableVector3 destination;
}
}