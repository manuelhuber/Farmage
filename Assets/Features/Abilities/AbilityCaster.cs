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
        _actions = new Observable<ActionEntry[]>(new ActionEntry[] { });

    private Dictionary<Ability, IAbilityExecutor> _executors = new Dictionary<Ability, IAbilityExecutor>();
    private MovementAgent _movementAgent;

    private void Start() {
        UpdateAbilities();
    }

    public IObservable<ActionEntry[]> GetActions() {
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
                    Active = executor.CanActivate, Image = ability.icon,
                    OnSelect = () => { executor.Activate(); }
                };
            })
            .ToArray();
        _actions.Set(actions);
    }
}
}