using System;
using System.Collections.Generic;
using System.Linq;
using Features.Ui.Actions;
using Features.Ui.UserInput;
using Features.Units.Common;
using Features.Units.Walkers.Abilities;
using Grimity.Data;
using UnityEngine;

namespace Features.Units.Walkers {
[RequireComponent(typeof(MovementAgent))]
public class Walker : MonoBehaviour, IInputReceiver, IHasActions {
    public List<Ability> abilities;

    private readonly Observable<ActionEntry[]>
        _actions = new Observable<ActionEntry[]>(new ActionEntry[] { });

    private Dictionary<Ability, IAbilityExecutor> _executors = new Dictionary<Ability, IAbilityExecutor>();
    private InputManager _inputManager;
    private MovementAgent _movementAgent;

    private void Start() {
        _inputManager = InputManager.Instance;
        _movementAgent = GetComponent<MovementAgent>();
        UpdateAbilities();
    }

    #region InputReceiver

    public event EventHandler YieldControl;

    public void OnKeyDown(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
        if (keys.Contains(KeyCode.Mouse0)) YieldControl?.Invoke(this, EventArgs.Empty);
    }

    public void OnKeyUp(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
        if (keys.Contains(KeyCode.Mouse1)) {
            _movementAgent.SetDestination(mouseLocation.Position);
        }
    }

    public void OnKeyPressed(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
    }

    #endregion

    public Grimity.Data.IObservable<ActionEntry[]> GetActions() {
        return _actions;
    }

    private void UpdateAbilities() {
        _executors = abilities.ToDictionary(ability => ability, ability => ability.AddExecutor(gameObject));
        UpdateActions();
    }

    private void UpdateActions() {
        var actions = _executors.Select(pair => {
                var (ability, executor) = pair;
                return new ActionEntry {
                    Active = executor.CanActivate, Image = ability.icon, OnSelect = () => {
                        executor.Activate();
                        _inputManager.RequestControlWithMemory(executor);
                    }
                };
            })
            .ToArray();

        _actions.Set(actions);
    }
}
}