using System.Collections.Generic;
using System.Linq;
using Features.Ui.Actions;
using Features.Units.Common;
using Grimity.Data;
using UnityEngine;

namespace Features.Abilities {
public class AbilityCaster : MonoBehaviour, IHasActions {
    public List<Ability> abilities;

    private readonly Observable<ActionEntryData[]>
        _actionsObservable = new Observable<ActionEntryData[]>(new ActionEntryData[] { });

    private Dictionary<IAbilityExecutor, ActionEntryData>
        _actionDict = new Dictionary<IAbilityExecutor, ActionEntryData>();

    private Dictionary<Ability, IAbilityExecutor> _executors =
        new Dictionary<Ability, IAbilityExecutor>();

    private MovementAgent _movementAgent;

    private void Awake() {
        UpdateAbilities();
    }

    private void Update() {
        foreach (var (executor, actionEntry) in _actionDict) {
            actionEntry.Active = executor.CanActivate;
            actionEntry.Cooldown = executor.CooldownRemaining;
        }
    }

    public IObservable<ActionEntryData[]> GetActions() {
        return _actionsObservable;
    }

    private void UpdateAbilities() {
        foreach (var (oldAbility, executor) in _executors) {
            if (!abilities.Contains(oldAbility)) Destroy(executor as Object);
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
                return new ActionEntryData {
                    Image = ability.icon,
                    OnSelect = () => { executor.Activate(); }
                };
            });
        _actionsObservable.Set(_actionDict.Values.ToArray());
    }
}
}