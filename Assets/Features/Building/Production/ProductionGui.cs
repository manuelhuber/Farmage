using System.Collections.Generic;
using Features.Building.UI;
using MonKey.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Building.Production {
public class ProductionGui : MonoBehaviour {
    public GameObject iconPrefab;

    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private GameObject root;
    private ProductionOption[] _defaultOptions;

    private void Start() {
        buildingManager.BuildingOptions.OnChange(options => _defaultOptions = options);
        ShowDefault();
    }

    public void ShowDefault() {
        BuildUi(_defaultOptions);
    }

    public void BuildUi(IEnumerable<ProductionOption> buildingOptions) {
        foreach (var child in root.transform.GetChildren()) {
            Destroy(child.gameObject);
        }

        foreach (var option in buildingOptions) {
            var button = Instantiate(iconPrefab, root.transform, false);
            button.GetComponent<Image>().sprite = option.Image;
            if (option.Buildable) {
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