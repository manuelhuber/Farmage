using System.Collections.Generic;
using Features.Ui.UserInput;
using Features.Units.Common;
using UnityEngine;

namespace Features.Units.Walkers {
[RequireComponent(typeof(MovementAgent))]
public class Walker : MonoBehaviour, IInputReceiver {
    private MovementAgent _movementAgent;

    private void Start() {
        _movementAgent = GetComponent<MovementAgent>();
    }

    #region InputReceiver

    public event YieldControlHandler YieldControl;

    public void OnKeyDown(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
        if (keys.Contains(KeyCode.Mouse0)) YieldControl?.Invoke(this, new YieldControlEventArgs());
    }

    public void OnKeyUp(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
        if (keys.Contains(KeyCode.Mouse1)) _movementAgent.SetDestination(mouseLocation.Position);
    }

    public void OnKeyPressed(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
    }

    #endregion
}
}