using System.Collections.Generic;
using Grimity.Data;
using MonKey.Extensions;
using UnityEngine;

namespace Features.Ui.Actions {
public class ActionsGui : MonoBehaviour {
    public ActionEntry iconPrefab;

    [SerializeField] private GameObject root;

    private readonly Dictionary<ActionEntryData, ActionEntry> _actions =
        new Dictionary<ActionEntryData, ActionEntry>();


    private void Update() {
        foreach (var (data, actionEntry) in _actions) {
            actionEntry.SetCooldown(data.Cooldown);
        }
    }

    public void BuildUi(IEnumerable<ActionEntryData> actions) {
        foreach (var child in root.transform.GetChildren()) {
            Destroy(child.gameObject);
        }

        _actions.Clear();

        foreach (var action in actions) {
            var actionEntry = Instantiate(iconPrefab, root.transform, false);

            actionEntry.OnClick(() => {
                if (action.Active) action.OnSelect.Invoke();
            });
            if (action.Image != null) {
                actionEntry.icon.sprite = action.Image;
            }

            _actions[action] = actionEntry;
        }
    }
}
}