using System.Collections.Generic;
using System.Linq;
using Features.Attacks.Damage;
using Features.Attacks.Trajectory;
using Features.Common;
using Features.Health;
using Features.Units.Common;
using Grimity.Data;
using UnityEngine;
using Utils;

namespace Features.Abilities.AutoAttack {
[RequireComponent(typeof(ITeam))]
public class AutoAttackExecutor : AbilityExecutor<AutoAttackAbility> {
    public IObservable<IReadOnlyList<Mortal>> EnemiesInRange => _enemiesInRangeObservable;

    private readonly List<Mortal> _enemiesInRange = new List<Mortal>();

    private readonly Observable<IReadOnlyList<Mortal>> _enemiesInRangeObservable =
        new Observable<IReadOnlyList<Mortal>>(new Mortal[] { });

    private Optional<Mortal> _currentTarget = Optional<Mortal>.NoValue();
    private Optional<MovementAgent> _movementAgent = Optional<MovementAgent>.NoValue();
    private Optional<Mortal> _priorityTarget = Optional<Mortal>.NoValue();
    private Team _team;

    private void Start() {
        var rangeCollider = RangeCollider.AddTo(gameObject, ability.range);
        rangeCollider.OnEnter(AddEnemy);
        rangeCollider.OnExit(RemoveEnemy);
        _movementAgent = GetComponent<MovementAgent>().AsOptional();
        _team = GetComponent<ITeam>().Team;
    }

    private void Update() {
        if (!IsOnCooldown) {
            Activate();
        }
    }

    public override void Activate() {
        if (!GetAttackTarget().HasValue) return;
        if (string.IsNullOrEmpty(ability.animationTrigger)) {
            DealDamage();
        } else {
            // This requires the animation not to be able to transition to itself
            // Otherwise it'll trigger it every frame
            AnimationHandler.SetTrigger(ability.animationTrigger);
        }
    }

    protected override void AnimationCallbackImpl() {
        // Reset to make sure the animation isn't played twice
        AnimationHandler.ResetTrigger(ability.animationTrigger);
        DealDamage();
    }


    private void DealDamage() {
        var target = GetAttackTarget();
        if (!target.HasValue) return;
        var damage = new Damage {Source = gameObject, Amount = ability.damage.amount};

        void Damage() {
            if (ability.damage.dealsAoE) {
                DamageUtil.DamageEnemies(target.Value.transform.position,
                    ability.damage.radius,
                    damage,
                    _team
                );
            } else {
                target.Value.TakeDamage(damage);
            }
        }

        if (ability.projectile != null) {
            var projectile = Instantiate(ability.projectile, transform.position, Quaternion.identity)
                .GetComponent<ProjectileMovement>();
            projectile.Go(target.Value.transform.position, ability.trajectory, Damage);
        } else {
            Damage();
        }

        CalculateNextCooldown();
    }

    private Optional<Mortal> GetAttackTarget() {
        var isMoving = _movementAgent.HasValue && _movementAgent.Value.IsMoving.Value;
        var canAttack = !isMoving || ability.shootDuringMove;
        if (!canAttack) return Optional<Mortal>.NoValue();

        if (_priorityTarget.HasValue && _enemiesInRange.Contains(_priorityTarget.Value)) {
            return _priorityTarget;
        }

        if (!_currentTarget.HasValue) {
            _currentTarget = _enemiesInRange.FirstOrDefault().AsOptional();
        }

        return _currentTarget;
    }

    public void SetPriorityTarget(Mortal priority) {
        _priorityTarget = priority.AsOptional();
    }

    public bool IsInRangeOf(Mortal target) {
        return _enemiesInRange.Contains(target);
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