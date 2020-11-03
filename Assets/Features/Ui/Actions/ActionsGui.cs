using System.Collections.Generic;
using Grimity.Data;
using MonKey.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Ui.Actions {
public class ActionsGui : MonoBehaviour {
    public GameObject iconPrefab;

    [SerializeField] private GameObject root;
    private readonly Dictionary<ActionEntry, Image> _actions = new Dictionary<ActionEntry, Image>();


    private void Update() {
        foreach (var (action, icon) in _actions) {
            icon.color = action.Active ? Color.white : Color.red;
        }
    }

    public void BuildUi(IEnumerable<ActionEntry> actions) {
        foreach (var child in root.transform.GetChildren()) {
            Destroy(child.gameObject);
        }

        _actions.Clear();

        foreach (var action in actions) {
            var button = Instantiate(iconPrefab, root.transform, false);
            var buttonClickedEvent = new Button.ButtonClickedEvent();
            buttonClickedEvent.AddListener(() => {
                if (action.Active) action.OnSelect.Invoke();
            });
            button.GetComponent<Button>().onClick = buttonClickedEvent;

            var icon = button.GetComponent<Image>();
            icon.sprite = action.Image;
            _actions[action] = icon;
        }
    }
}
}