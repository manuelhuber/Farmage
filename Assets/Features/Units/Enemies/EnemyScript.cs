using System.Linq;
using Features.Health;
using Features.Units.Common;
using Grimity.Actions;
using Grimity.Collections;
using UnityEngine;

namespace Features.Units.Enemies {
public class EnemyScript : MonoBehaviour {
    private IntervaledAction _attack;
    private MovementAgent _movementAgent;
    private GameObject[] _targets = new GameObject[0];
    private Mortal _victim;
    public float attackSpeed;
    public int damage = 5;


    private void Awake() {
        _movementAgent = GetComponent<MovementAgent>();
        _attack = gameObject.AddComponent<IntervaledAction>();
        _attack.interval = attackSpeed;
        _attack.action = () => {
            _victim.TakeDamage(damage);
            return true;
        };
    }

    private void Start() {
        FindNewTarget();
    }

    public void SetTargets(GameObject[] targets) {
        _targets = targets;
    }

    private void OnTriggerExit(Collider other) {
        if (_victim == null) return;
        if (other.gameObject != _victim.gameObject) return;
        StopAttack();
        _movementAgent.SetDestination(_victim.transform.position, true);
    }

    private void OnTriggerEnter(Collider other) {
        var destructible = other.gameObject.GetComponent<Mortal>();
        if (destructible == null || destructible.team == Team.Aliens) return;
        transform.LookAt(destructible.transform);
        _movementAgent.IsStopped = true;
        _victim = destructible;
        _attack.IsRunning = true;
        _victim.onDeath.AddListener(StopAttack);
    }

    private void StopAttack() {
        _attack.IsRunning = false;
        _movementAgent.IsStopped = false;
        FindNewTarget();
    }

    private void FindNewTarget() {
        if (_targets.Length == 0) return;
        if (_targets.All(t => t == null)) return;
        var target = _targets.GetRandomElement();
        _movementAgent.SetDestination(target.transform.position, true);
    }
}
}