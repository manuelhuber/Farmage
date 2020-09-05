using UnityEngine;

namespace Features.Ui.Selection {
public class Selectable : MonoBehaviour {
    public Sprite icon;
    public string displayName;
    public GameObject uiDetailPrefab;

    // Start is called before the first frame update
    private void Start() {
        SelectionManager.Instance.Register(this);
    }
}
}