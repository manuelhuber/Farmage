using UnityEngine;

namespace Features.Units.Common {
public class Unit : MonoBehaviour {
    private MovementAgent _agent;

    // Start is called before the first frame update
    private void Start() {
        _agent = GetComponent<MovementAgent>();
        UnitControl.Instance.Register(this);
    }

    public void SetTarget(Vector3 destination) {
        _agent.SetDestination(destination);
    }
}
}