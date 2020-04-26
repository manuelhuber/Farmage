using UnityEngine;
using UnityEngine.AI;

namespace Features.Units {
public class Unit : MonoBehaviour {
    private NavMeshAgent _agent;

    // Start is called before the first frame update
    private void Start() {
        _agent = GetComponent<NavMeshAgent>();
        UnitControl.Instance.Register(this);
    }

    public void setTarget(Vector3 destination) {
        _agent.SetDestination(destination);
    }
}
}