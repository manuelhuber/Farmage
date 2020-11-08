using System;
using System.Collections.Generic;
using System.Linq;
using Features.Common;
using Features.Health;
using Features.Save;
using Features.Time;
using Features.Units.Common;
using Grimity.Actions;
using Grimity.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace Features.Units.Enemies {
public class EnemyScript : MonoBehaviour, ISavableComponent<EnemyData> {
    public float attackSpeed;
    public int damage = 5;
    private PeriodicalAction _attack;
    private MovementAgent _movementAgent;
    private List<Mortal> _targets = new List<Mortal>();
    private GameTime _time;
    [CanBeNull] private Mortal _victim;


    private void Awake() {
        _time = GameTime.Instance;
        _movementAgent = GetComponent<MovementAgent>();
        _attack = gameObject.AddComponent<PeriodicalAction>();
        _attack.interval = attackSpeed;
        _attack.getTime = () => _time.Time;
        _attack.action = () => {
            if (_victim != null) _victim.TakeDamage(new Damage {Amount = damage});
            return true;
        };
        var rangeCollider = RangeCollider.AddTo(gameObject, 1.2f);
        rangeCollider.OnEnter(OnEnterRange);
        rangeCollider.OnExit(OnExitRange);
    }

    private void Start() {
        FindNewTarget();
    }

    private void OnEnterRange(Collider other) {
        var destructible = other.gameObject.GetComponent<Mortal>();
        if (destructible == null || destructible.Team == Team.Aliens) return;
        transform.LookAt(destructible.transform);
        _movementAgent.IsStopped = true;
        SetTarget(destructible);
    }

    private void OnExitRange(Collider other) {
        var victim = _victim;
        if (victim == null) return;
        if (other.gameObject != victim.gameObject) return;
        StopAttack();
        _movementAgent.SetDestination(victim.transform.position, true);
    }

    public void SetTargets(List<Mortal> targets) {
        _targets = targets;
        foreach (var target in _targets) {
            target.onDeath.AddListener(() => _targets.Remove(target));
        }
    }

    private void SetTarget(Mortal destructible) {
        _victim = destructible;
        _attack.IsRunning = true;
        destructible.onDeath.AddListener(() => {
            _victim = null;
            StopAttack();
        });
    }

    private void StopAttack() {
        _attack.IsRunning = false;
        _movementAgent.IsStopped = false;
        FindNewTarget();
    }

    private void FindNewTarget() {
        if (_targets.Count == 0) return;
        if (_targets.All(t => t == null)) return;
        var target = _targets.GetRandomElement();
        _movementAgent.SetDestination(target.transform.position, true);
    }

    #region Save

    public string SaveKey => "EnemyController";

    public EnemyData Save() {
        var victim = _victim;
        return new EnemyData {
            target = victim == null ? "" : victim.getSaveID(),
            targets = _targets.Select(mortal => mortal.getSaveID()).ToArray(),
            nextAttack = _attack.NextExecution
        };
    }

    public void Load(EnemyData data, IReadOnlyDictionary<string, GameObject> objects) {
        _attack.SetNextExecution(data.nextAttack);
        _targets = data.targets.Select(t => objects[t].GetComponent<Mortal>()).ToList();

        var target = objects.getBySaveID(data.target);
        if (target != null) {
            SetTarget(target.GetComponent<Mortal>());
        }
    }

    #endregion
}

[Serializable]
public struct EnemyData {
    public string target;
    public string[] targets;
    public float nextAttack;
}
}