using System.Collections.Generic;
using Features.Health;
using Features.Ui.Actions;
using Features.Ui.UserInput;
using Features.Units.Common;
using Grimity.Data;
using UnityEngine;

namespace Features.Units.Walkers {
[RequireComponent(typeof(AdvancedMovementController))]
[RequireComponent(typeof(ITeam))]
public class Walker : MonoBehaviour, IOnKeyDown, IOnKeyUp, IOnReceiveControl, IHasActions {
    private readonly Observable<ActionEntryData[]> _actionsObservable =
        new Observable<ActionEntryData[]>(new ActionEntryData[] { });

    private AdvancedMovementController _movementController;
    private bool _selectingAttackMove;
    private Team _team;

    private void Start() {
        _movementController = GetComponent<AdvancedMovementController>();
        _team = GetComponent<ITeam>().Team;

        _actionsObservable.Set(new[] {
            new ActionEntryData {OnSelect = () => _movementController.Stop()},
            new ActionEntryData {OnSelect = () => _selectingAttackMove = true}
        });
    }

    #region InputReceiver

    public event YieldControlHandler YieldControl;

    public void OnReceiveControl() {
        _selectingAttackMove = false;
    }

    public void OnKeyDown(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
        if (keys.Contains(KeyCode.Escape)) {
            if (_selectingAttackMove) {
                _selectingAttackMove = false;
            } else {
                YieldControl?.Invoke(this, new YieldControlEventArgs(true));
            }
        }

        if (keys.Contains(KeyCode.Mouse0)) YieldControl?.Invoke(this, new YieldControlEventArgs());
    }

    public void OnKeyUp(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
        if (!keys.Contains(KeyCode.Mouse1)) return;
        _movementController.Stop();
        var clickTarget = mouseLocation.Collision.GetComponent<Mortal>();
        if (clickTarget != null && clickTarget.Team != _team) {
            _movementController.ChaseTarget(clickTarget.transform);
        } else if (_selectingAttackMove) {
            _movementController.AttackMoveTo(mouseLocation.Position);
        } else {
            _movementController.MoveTo(mouseLocation.Position);
        }

        _selectingAttackMove = false;
    }

    #endregion

    public IObservable<ActionEntryData[]> GetActions() {
        return _actionsObservable;
    }
}
}