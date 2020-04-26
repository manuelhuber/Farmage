using System.Linq;
using Features.Units;
using Grimity.Actions;
using Grimity.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Features.Enemies {
public class EnemyScript : MonoBehaviour {
    private IntervaledAction _attack;
    private NavMeshAgent _navMeshAgent;
    private GameObject[] _targets = new GameObject[0];
    private Mortal _victim;
    public float attackSpeed;
    public int damage = 5;


    private void Start() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _attack = gameObject.AddComponent<IntervaledAction>();
        _attack.interval = attackSpeed;
        _attack.action = () => {
            _victim.TakeDamage(damage);
            return true;
        };
        FindNewTarget();
    }

    public void setTargets(GameObject[] targets) {
        _targets = targets;
    }

    private void OnTriggerExit(Collider other) {
        if (_victim == null) return;
        if (other.gameObject != _victim.gameObject) return;
        StopAttack();
        _navMeshAgent.SetDestination(_victim.transform.position);
    }

    private void OnTriggerEnter(Collider other) {
        var destructible = other.gameObject.GetComponent<Mortal>();
        if (destructible == null || destructible.team == Team.Aliens) return;
        transform.LookAt(destructible.transform);
        _navMeshAgent.isStopped = true;
        _victim = destructible;
        _attack.IsRunning = true;
        _victim.onDeath.AddListener(StopAttack);
    }

    private void StopAttack() {
        _attack.IsRunning = false;
        _navMeshAgent.isStopped = false;
        FindNewTarget();
    }

    private void FindNewTarget() {
        if (_targets.Length == 0) return;
        if (_targets.All(t => t == null)) return;
        var target = _targets.GetRandomElement();
        _navMeshAgent.SetDestination(target.transform.position);
    }
}
}