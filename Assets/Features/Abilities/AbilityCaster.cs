using System.Collections.Generic;
using System.Linq;
using Features.Ui.Actions;
using Features.Units.Common;
using Grimity.Data;
using UnityEngine;

namespace Features.Abilities {
public class AbilityCaster : MonoBehaviour, IHasActions {
    public List<Ability> abilities;

    private readonly Observable<ActionEntry[]>
        _actionsObservable = new Observable<ActionEntry[]>(new ActionEntry[] { });

    private Dictionary<AbilityExecutor, ActionEntry>
        _actionDict = new Dictionary<AbilityExecutor, ActionEntry>();

    private Dictionary<Ability, AbilityExecutor> _executors = new Dictionary<Ability, AbilityExecutor>();
    private MovementAgent _movementAgent;

    private void Start() {
        UpdateAbilities();
    }

    private void Update() {
        foreach (var (executor, actionEntry) in _actionDict) {
            actionEntry.Active = executor.CanActivate;
        }
    }

    public IObservable<ActionEntry[]> GetActions() {
        return _actionsObservable;
    }

    private void UpdateAbilities() {
        foreach (var (oldAbility, executor) in _executors) {
            if (!abilities.Contains(oldAbility)) Destroy(executor);
        }

        _executors = abilities.ToDictionary(ability => ability,
            ability => _executors.ContainsKey(ability)
                ? _executors[ability]
                : ability.AddExecutor(gameObject));

        UpdateActions();
    }

    private void UpdateActions() {
        _actionDict = _executors.ToDictionary(pair => pair.Value,
            pair => {
                var (ability, executor) = pair;
                return new ActionEntry {
                    Active = executor.CanActivate,
                    Image = ability.icon,
                    OnSelect = () => { executor.Activate(); }
                };
            });
        _actionsObservable.Set(_actionDict.Values.ToArray());
    }
}
}