using System.Collections.Generic;
using MonKey.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Building.UI {
public class BuildingGui : MonoBehaviour {
    public GameObject iconPrefab;

    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private GameObject root;

    private void Start() {
        buildingManager.BuildingOptions.OnChange(BuildUi);
    }

    private void BuildUi(BuildingOption[] buildingOptions) {
        foreach (var child in root.transform.GetChildren()) {
            Destroy(child.gameObject);
        }

        foreach (var option in buildingOptions) {
            var button = Instantiate(iconPrefab, root.transform, false);
            button.GetComponent<Image>().sprite = option.Entry.image;
            if (option.Buildable) {
                var buttonClickedEvent = new Button.ButtonClickedEvent();
                var option1 = option;
                buttonClickedEvent.AddListener(() => { option1.OnSelect.Invoke(); });
                button.GetComponent<Button>().onClick = buttonClickedEvent;
                button.GetComponent<Image>().color = Color.white;
            } else {
                button.GetComponent<Image>().color = Color.red;
                button.GetComponent<Button>().onClick = null;
            }
        }
    }
}
}