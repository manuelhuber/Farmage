using UnityEngine;
using UnityEngine.UI;

namespace Features.Resources {
public class ResourceGui : MonoBehaviour {
    [SerializeField] private Text text;
    private ResourceManager _resourceManager;

    private void Awake() {
        _resourceManager = ResourceManager.Instance;
    }

    private void Start() {
        _resourceManager.Have.OnChange(UpdateUi);
    }

    private void UpdateUi(Cost cost) {
        text.text = $"{cost.cash}$";
    }
}
}