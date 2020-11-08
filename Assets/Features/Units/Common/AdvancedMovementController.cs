using Features.Abilities.AutoAttack;
using Features.Health;
using Grimity.Data;
using UnityEngine;

namespace Features.Units.Common {
internal enum MoveMode {
    Move,
    AttackMove,
    Chase
}

[RequireComponent(typeof(AutoAttackExecutor))]
[RequireComponent(typeof(MovementAgent))]
public class AdvancedMovementController : MonoBehaviour {
    private bool _anyEnemiesInRange;
    private Transform _huntTarget;
    private Optional<Mortal> _huntTargetMortal = Optional<Mortal>.NoValue();
    private MoveMode _moveMode = MoveMode.Move;
    private MovementAgent _movementAgent;
    private bool _selectingAttackMove;
    private AutoAttackExecutor _autoAttack;

    private void Start() {
        _movementAgent = GetComponent<MovementAgent>();

        _autoAttack = GetComponent<AutoAttackExecutor>();
        _autoAttack.EnemiesInRange.OnChange(enemies => _anyEnemiesInRange = enemies.Count > 0);
    }

    private void Update() {
        var canAttack = _huntTargetMortal.HasValue && _autoAttack.IsInRangeOf(_huntTargetMortal.Value);
        switch (_moveMode) {
            case MoveMode.AttackMove:
                _movementAgent.IsStopped = _anyEnemiesInRange;
                break;
            case MoveMode.Chase when canAttack:
                _movementAgent.IsStopped = true;
                break;
            case MoveMode.Chase:
                var huntTargetPosition = _huntTarget.position;
                if (_movementAgent.CurrentDestination == huntTargetPosition) return;
                _movementAgent.SetDestination(huntTargetPosition, false);
                _movementAgent.IsStopped = false;
                break;
        }
    }

    public void Stop() {
        if (_huntTargetMortal.HasValue) _huntTargetMortal.Value.onDeath.RemoveListener(Stop);
        _movementAgent.AbandonDestination();
    }

    public void AttackMoveTo(Vector3 pos) {
        _moveMode = MoveMode.AttackMove;
        _movementAgent.SetDestination(pos, false);
    }

    public void MoveTo(Vector3 pos) {
        _movementAgent.SetDestination(pos, false);
        _moveMode = MoveMode.Move;
    }

    public void ChaseTarget(Transform target) {
        _huntTarget = target;
        _moveMode = MoveMode.Chase;
        _huntTargetMortal = target.GetComponent<Mortal>().AsOptional();
        if (!_huntTargetMortal.HasValue) return;
        _autoAttack.SetPriorityTarget(_huntTargetMortal.Value);
        _huntTargetMortal.Value.onDeath.AddListener(Stop);
    }
}
}