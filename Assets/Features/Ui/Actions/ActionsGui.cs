using System.Collections.Generic;
using MonKey.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Ui.Actions {
public class ActionsGui : MonoBehaviour {
    public GameObject iconPrefab;

    [SerializeField] private GameObject root;

    public void BuildUi(IEnumerable<ActionEntry> buildingOptions) {
        foreach (var child in root.transform.GetChildren()) {
            Destroy(child.gameObject);
        }

        foreach (var option in buildingOptions) {
            var button = Instantiate(iconPrefab, root.transform, false);
            button.GetComponent<Image>().sprite = option.Image;
            if (option.Active) {
                var buttonClickedEvent = new Button.ButtonClickedEvent();
                buttonClickedEvent.AddListener(() => { option.OnSelect.Invoke(); });
                button.GetComponent<Button>().onClick = buttonClickedEvent;
                button.GetComponent<Image>().color = Color.white;
            } else {
                button.GetComponent<Image>().color = Color.red;
            }
        }
    }
}
}