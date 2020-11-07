using System.Collections.Generic;
using Features.Ui.Actions;
using Features.Ui.UserInput;
using Features.Units.Common;
using Grimity.Data;
using UnityEngine;

namespace Features.Units.Walkers {
[RequireComponent(typeof(MovementAgent))]
public class Walker : MonoBehaviour, IInputReceiver, IHasActions {
    private readonly Observable<ActionEntryData[]> _actionsObservable =
        new Observable<ActionEntryData[]>(new ActionEntryData[] { });

    private MovementAgent _movementAgent;

    private void Start() {
        _movementAgent = GetComponent<MovementAgent>();
        _actionsObservable.Set(new[] {
            new ActionEntryData {
                Active = true, Cooldown = 0, OnSelect = () => _movementAgent.AbandonDestination()
            }
        });
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

    public IObservable<ActionEntryData[]> GetActions() {
        return _actionsObservable;
    }
}
}