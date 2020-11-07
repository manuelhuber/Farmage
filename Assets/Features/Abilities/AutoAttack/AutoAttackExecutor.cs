using System.Collections.Generic;
using System.Linq;
using Features.Common;
using Features.Health;
using Features.Units.Common;
using Grimity.Data;
using UnityEngine;

namespace Features.Abilities.AutoAttack {
public class AutoAttackExecutor : AbilityExecutor<AutoAttackAbility> {
    public IObservable<IReadOnlyList<Mortal>> EnemiesInRange => _enemiesInRangeObservable;

    private readonly List<Mortal> _enemiesInRange = new List<Mortal>();

    private readonly Observable<IReadOnlyList<Mortal>> _enemiesInRangeObservable =
        new Observable<IReadOnlyList<Mortal>>(new Mortal[] { });

    private Optional<Mortal> _currentTarget = Optional<Mortal>.NoValue();
    private Optional<MovementAgent> _movementAgent;

    private void Update() {
        if (!IsOnCooldown) {
            Attack();
        }
    }

    public override void Activate() {
    }

    private void Attack() {
        var isMoving = _movementAgent.HasValue && !_movementAgent.Value.IsStopped &&
                       !_movementAgent.Value.HasArrived;
        var canAttack = !isMoving || ability.shootDuringMove;
        var noTargetAvailable = !_currentTarget.HasValue && !FindNewTarget();
        if (!canAttack || noTargetAvailable) return;

        _currentTarget.Value.TakeDamage(new Damage {Amount = ability.damage});
        CalculateNextCooldown();
    }

    private bool FindNewTarget() {
        _currentTarget = _enemiesInRange.FirstOrDefault().AsOptional();
        return _currentTarget.HasValue;
    }

    protected override void InitImpl() {
        var rangeCollider = RangeCollider.AddTo(gameObject, ability.range);
        rangeCollider.OnEnter(AddEnemy);
        rangeCollider.OnExit(RemoveEnemy);
        _movementAgent = GetComponent<MovementAgent>().AsOptional();
    }

    private void AddEnemy(Collider col) {
        var mortal = col.GetComponent<Mortal>();
        if (mortal == null || mortal.team != Team.Aliens) return;
        mortal.onDeath.AddListener(() => RemoveEnemy(mortal));
        _enemiesInRange.Add(mortal);
        _enemiesInRangeObservable.Set(_enemiesInRange.ToArray());
    }

    private void RemoveEnemy(Collider col) {
        RemoveEnemy(col.GetComponent<Mortal>());
    }

    private void RemoveEnemy(Mortal leavingEnemy) {
        _enemiesInRange.Remove(leavingEnemy);
        if (_currentTarget.HasValue && _currentTarget.Value == leavingEnemy) {
            _currentTarget = Optional<Mortal>.NoValue();
        }

        _enemiesInRangeObservable.Set(_enemiesInRange.ToArray());
    }
}
}