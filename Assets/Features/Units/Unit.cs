using UnityEngine;
using UnityEngine.AI;

namespace Features.Units {
public class Unit : MonoBehaviour {
    private MovementAgent _agent;

    // Start is called before the first frame update
    private void Start() {
        _agent = GetComponent<MovementAgent>();
        UnitControl.Instance.Register(this);
    }

    public void setTarget(Vector3 destination) {
        _agent.SetDestination(destination);
    }
}
}