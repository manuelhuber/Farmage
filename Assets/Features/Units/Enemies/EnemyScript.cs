using System;
using System.Collections.Generic;
using System.Linq;
using Features.Health;
using Features.Save;
using Features.Time;
using Features.Units.Common;
using Grimity.Actions;
using Grimity.Collections;
using JetBrains.Annotations;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;

namespace Features.Units.Enemies {
public class EnemyScript : MonoBehaviour, ISavableComponent {
    private PeriodicalAction _attack;
    private MovementAgent _movementAgent;
    private List<Mortal> _targets = new List<Mortal>();
    [CanBeNull] private Mortal _victim;
    public float attackSpeed;
    public int damage = 5;
    private GameTime _time;


    private void Awake() {
        _time = GameTime.Instance;
        _movementAgent = GetComponent<MovementAgent>();
        _attack = gameObject.AddComponent<PeriodicalAction>();
        _attack.interval = attackSpeed;
        _attack.getTime = () => _time.Time;
        _attack.action = () => {
            if (_victim != null) _victim.TakeDamage(damage);
            return true;
        };
    }

    private void Start() {
        FindNewTarget();
    }

    public void SetTargets(List<Mortal> targets) {
        _targets = targets;
        foreach (var target in _targets) {
            target.onDeath.AddListener(() => _targets.Remove(target));
        }
    }

    private void OnTriggerExit(Collider other) {
        var victim = _victim;
        if (victim == null) return;
        if (other.gameObject != victim.gameObject) return;
        StopAttack();
        _movementAgent.SetDestination(victim.transform.position, true);
    }

    private void OnTriggerEnter(Collider other) {
        var destructible = other.gameObject.GetComponent<Mortal>();
        if (destructible == null || destructible.team == Team.Aliens) return;
        transform.LookAt(destructible.transform);
        _movementAgent.IsStopped = true;
        SetTarget(destructible);
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

    public string SaveKey => "EnemyController";

    public string Save() {
        var victim = _victim;
        return new EnemyData() {
            target = victim == null ? "" : victim.getSaveID(),
            targets = _targets.Select(mortal => mortal.getSaveID()).ToArray(),
            nextAttack = _attack.NextExecution
        }.ToJson();
    }

    public void Load(string rawData, IReadOnlyDictionary<string, GameObject> objects) {
        var data = rawData.FromJson<EnemyData>();
        _attack.SetNextExecution(data.nextAttack);
        SetTarget(objects[data.target].GetComponent<Mortal>());
        _targets = data.targets.Select(t => objects[t].GetComponent<Mortal>()).ToList();
    }
}

[Serializable]
struct EnemyData {
    public string target;
    public string[] targets;
    public float nextAttack;
}
}