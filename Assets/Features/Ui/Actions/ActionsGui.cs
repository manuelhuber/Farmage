using System;
using System.Collections.Generic;
using Features.Ui.UserInput;
using MonKey.Extensions;
using UnityEngine;

namespace Features.Ui.Actions {
public class ActionsGui : MonoBehaviour, IOnKeyUp {
    public ActionEntry iconPrefab;

    [SerializeField] private GameObject root;


    private readonly List<Tuple<ActionEntryData, ActionEntry>> _actions =
        new List<Tuple<ActionEntryData, ActionEntry>>();

    private readonly KeyCode[] _hotkeys = {
        KeyCode.Q,
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.E
    };


    private void Start() {
        InputManager.Instance.RegisterForPermanentInput(this);
    }

    private void Update() {
        foreach (var (data, actionEntry) in _actions) {
            actionEntry.SetCooldown(data.Cooldown);
        }
    }

    #region InputReceiver

    public event YieldControlHandler YieldControl;

    public void OnKeyUp(HashSet<KeyCode> keys, HashSet<KeyCode> pressedKeys, MouseLocation mouseLocation) {
        for (var i = 0; i < Math.Min(_actions.Count, _hotkeys.Length); i++) {
            if (keys.Contains(_hotkeys[i])) {
                InvokeAction(_actions[i].Item1);
            }
        }
    }

    #endregion

    public void BuildUi(IEnumerable<ActionEntryData> actions) {
        foreach (var child in root.transform.GetChildren()) {
            Destroy(child.gameObject);
        }

        _actions.Clear();

        foreach (var action in actions) {
            var actionEntry = Instantiate(iconPrefab, root.transform, false);

            actionEntry.OnClick(() => InvokeAction(action));
            if (action.Image != null) {
                actionEntry.icon.sprite = action.Image;
            }

            _actions.Add(new Tuple<ActionEntryData, ActionEntry>(action, actionEntry));
        }
    }

    private static void InvokeAction(ActionEntryData action) {
        if (action.Active) action.OnSelect.Invoke();
    }
}
}