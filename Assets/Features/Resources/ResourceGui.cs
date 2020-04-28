using UnityEngine;
using UnityEngine.UI;

namespace Features.Resources {
public class ResourceGui : MonoBehaviour {
    private ResourceManager _resourceManager;
    [SerializeField] private Text text;

    private void Awake() {
        _resourceManager = ResourceManager.Instance;
    }

    private void Start() {
        _resourceManager.Have.OnChange(UpdateUI);
    }

    private void UpdateUI(Cost cost) {
        text.text = $"{cost.cash}$";
    }
}
}