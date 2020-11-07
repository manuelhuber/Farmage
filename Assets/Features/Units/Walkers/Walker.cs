using System.Collections.Generic;
using Features.Abilities.AutoAttack;
using Features.Ui.Actions;
using Features.Ui.UserInput;
using Features.Units.Common;
using Grimity.Data;
using UnityEngine;

namespace Features.Units.Walkers {
[RequireComponent(typeof(MovementAgent))]
public class Walker : MonoBehaviour, IOnKeyDown, IOnKeyUp, IOnReceiveControl, IHasActions {
    private readonly Observable<ActionEntryData[]> _actionsObservable =
        new Observable<ActionEntryData[]>(new ActionEntryData[] { });

    private bool _enemiesInRange;


    private bool _isOnAttackMove;
    private MovementAgent _movementAgent;
    private bool _selectingAttackMoveLocation;

    private void Start() {
        _movementAgent = GetComponent<MovementAgent>();
        _actionsObservable.Set(new[] {
            new ActionEntryData {
                Active = true, Cooldown = 0, OnSelect = () => _movementAgent.AbandonDestination()
            },
            new ActionEntryData {
                Active = true, Cooldown = 0, OnSelect = () => _selectingAttackMoveLocation = true
            }
        });
        var autoAttack = GetComponent<AutoAttackExecutor>();
        if (autoAttack != null) {
            autoAttack.EnemiesInRange.OnChange(enemies => _enemiesInRange = enemies.Count > 0);
        }
    }

    private void Update() {
        if (_isOnAttackMove) _movementAgent.IsStopped = _enemiesInRange;
    }

    #region InputReceiver

    public event YieldControlHandler YieldControl;

    public void OnReceiveControl() {
        _selectingAttackMoveLocation = false;
    }

    public void OnKeyDown(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
        if (keys.Contains(KeyCode.Escape)) {
            if (_selectingAttackMoveLocation) {
                _selectingAttackMoveLocation = false;
            } else {
                YieldControl?.Invoke(this, new YieldControlEventArgs(true));
            }
        }

        if (keys.Contains(KeyCode.Mouse0)) YieldControl?.Invoke(this, new YieldControlEventArgs());
    }

    public void OnKeyUp(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
        if (keys.Contains(KeyCode.Mouse1)) {
            _movementAgent.SetDestination(mouseLocation.Position);
            _movementAgent.IsStopped = false;
            _isOnAttackMove = _selectingAttackMoveLocation;
            _selectingAttackMoveLocation = false;
        }
    }

    #endregion

    public IObservable<ActionEntryData[]> GetActions() {
        return _actionsObservable;
    }
}
}