using System.Collections.Generic;
using System.Linq;
using Features.Common;
using Features.Health;
using Features.Units.Common;
using Grimity.Data;
using UnityEngine;

namespace Features.Abilities.AutoAttack {
[RequireComponent(typeof(ITeam))]
public class AutoAttackExecutor : AbilityExecutor<AutoAttackAbility> {
    public IObservable<IReadOnlyList<Mortal>> EnemiesInRange => _enemiesInRangeObservable;

    private readonly List<Mortal> _enemiesInRange = new List<Mortal>();

    private readonly Observable<IReadOnlyList<Mortal>> _enemiesInRangeObservable =
        new Observable<IReadOnlyList<Mortal>>(new Mortal[] { });

    private Optional<Mortal> _currentTarget = Optional<Mortal>.NoValue();
    private Optional<Mortal> _priorityTarget = Optional<Mortal>.NoValue();
    private Optional<MovementAgent> _movementAgent = Optional<MovementAgent>.NoValue();
    private Team _team;

    private void Update() {
        if (!IsOnCooldown) {
            Activate();
        }
    }

    private void Start() {
        var rangeCollider = RangeCollider.AddTo(gameObject, ability.range);
        rangeCollider.OnEnter(AddEnemy);
        rangeCollider.OnExit(RemoveEnemy);
        _movementAgent = GetComponent<MovementAgent>().AsOptional();
        _team = GetComponent<ITeam>().Team;
    }

    public override void Activate() {
        if (string.IsNullOrEmpty(ability.animationTrigger)) {
            DealDamage();
        } else {
            _animationHandler.Trigger(ability.animationTrigger);
        }
    }

    protected override void AnimationCallbackImpl() {
        DealDamage();
    }


    private void DealDamage() {
        var isMoving = _movementAgent.HasValue && _movementAgent.Value.IsMoving;
        var canAttack = !isMoving || ability.shootDuringMove;
        var target = GetTarget();
        if (!canAttack || !target.HasValue) return;
        target.Value.TakeDamage(new Damage {Source = gameObject, Amount = ability.damage});
        CalculateNextCooldown();
    }

    public void SetPriorityTarget(Mortal priority) {
        _priorityTarget = priority.AsOptional();
    }

    public bool IsInRangeOf(Mortal target) {
        return _enemiesInRange.Contains(target);
    }

    private Optional<Mortal> GetTarget() {
        if (_priorityTarget.HasValue && _enemiesInRange.Contains(_priorityTarget.Value)) {
            return _priorityTarget;
        }

        if (!_currentTarget.HasValue) {
            _currentTarget = _enemiesInRange.FirstOrDefault().AsOptional();
        }

        return _currentTarget;
    }

    private void AddEnemy(Collider col) {
        var mortal = col.GetComponent<Mortal>();
        if (mortal == null || mortal.Team == _team) return;
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