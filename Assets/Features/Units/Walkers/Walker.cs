using System.Collections.Generic;
using Features.Abilities.AutoAttack;
using Features.Health;
using Features.Ui.Actions;
using Features.Ui.UserInput;
using Features.Units.Common;
using Grimity.Data;
using UnityEngine;

namespace Features.Units.Walkers {
internal enum MoveMode {
    Move,
    AttackMove,
    Hunt
}

[RequireComponent(typeof(AutoAttackExecutor))]
[RequireComponent(typeof(MovementAgent))]
[RequireComponent(typeof(ITeam))]
public class Walker : MonoBehaviour, IOnKeyDown, IOnKeyUp, IOnReceiveControl, IHasActions {
    private readonly Observable<ActionEntryData[]> _actionsObservable =
        new Observable<ActionEntryData[]>(new ActionEntryData[] { });

    private bool _anyEnemiesInRange;
    private Mortal _huntTarget;

    private MoveMode _moveMode = MoveMode.Move;
    private MovementAgent _movementAgent;
    private bool _selectingAttackMove;
    private Team _team;
    private AutoAttackExecutor _autoAttack;

    private void Start() {
        _movementAgent = GetComponent<MovementAgent>();
        _team = GetComponent<ITeam>().Team;

        _autoAttack = GetComponent<AutoAttackExecutor>();
        _autoAttack.EnemiesInRange.OnChange(enemies => _anyEnemiesInRange = enemies.Count > 0);

        _actionsObservable.Set(new[] {
            new ActionEntryData {OnSelect = Stop},
            new ActionEntryData {OnSelect = () => _selectingAttackMove = true}
        });
    }

    private void Stop() {
        if (_huntTarget != null) _huntTarget.onDeath.RemoveListener(Stop);
        _movementAgent.AbandonDestination();
        _moveMode = MoveMode.Move;
    }

    private void Update() {
        if (_moveMode == MoveMode.AttackMove) {
            _movementAgent.IsStopped = _anyEnemiesInRange;
        } else if (_moveMode == MoveMode.Hunt) {
            if (_autoAttack.IsInRangeOf(_huntTarget)) {
                _movementAgent.IsStopped = true;
            } else {
                var huntTargetPosition = _huntTarget.transform.position;
                if (_movementAgent.CurrentDestination == huntTargetPosition) return;
                _movementAgent.SetDestination(huntTargetPosition);
                _movementAgent.IsStopped = false;
            }
        }
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
        Stop();
        _movementAgent.SetDestination(mouseLocation.Position);
        _movementAgent.IsStopped = false;
        var clickTarget = mouseLocation.Collision.GetComponent<Mortal>();
        if (clickTarget != null && clickTarget.Team != _team) {
            _huntTarget = clickTarget;
            _moveMode = MoveMode.Hunt;
            _autoAttack.SetPriorityTarget(clickTarget);
            clickTarget.onDeath.AddListener(Stop);
        } else {
            _moveMode = _selectingAttackMove ? MoveMode.AttackMove : MoveMode.Move;
        }

        _selectingAttackMove = false;
    }

    #endregion

    public IObservable<ActionEntryData[]> GetActions() {
        return _actionsObservable;
    }
}
}