using System;
using UnityEngine;

namespace Features.Attacks.Trajectory {
public class ProjectileMovement : MonoBehaviour {
    private Action _onArrival;
    private float _progress;
    private Rigidbody _rigidbody;
    private Vector3 _target;
    private float _totalDistance;
    private Trajectory _trajectory;

    private void FixedUpdate() {
        var trans = transform;
        var currentDistance = DistanceAsTheCrowFlies(trans.position, _target);
        var progress = 1 - currentDistance / _totalDistance;

        var speedFactor = _trajectory.speed.Evaluate(progress);
        var speed = speedFactor * _trajectory.baseSpeed;

        var heightFactor = _trajectory.height.Evaluate(progress);
        var height = _totalDistance * heightFactor;

        var newPosition = transform.position;
        newPosition.y = height;
        var movementDirection = (_target - newPosition).normalized;
        newPosition += movementDirection * speed;
        trans.LookAt(newPosition);
        trans.position = newPosition;
        if (!(currentDistance < speed)) return;
        if (_trajectory.impactFx != null) {
            var impactFX = Instantiate(_trajectory.impactFx, _target, Quaternion.identity);
            // Can't figure out how to calculate lifetime of particle system so let's use a large value
            const int timeToLife = 10;
            Destroy(impactFX, timeToLife);
        }

        _onArrival.Invoke();
        Destroy(gameObject);
    }

    public void Go(Vector3 target, Trajectory trajectory, Action onArrival) {
        _onArrival = onArrival;
        _trajectory = trajectory;
        _target = target;

        _totalDistance = DistanceAsTheCrowFlies(transform.position, target);
    }

    private static float DistanceAsTheCrowFlies(Vector3 a, Vector3 b) {
        a.y = 0;
        b.y = 0;
        return (a - b).magnitude;
    }
}
}