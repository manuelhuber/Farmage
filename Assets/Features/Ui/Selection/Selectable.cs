using System;
using UnityEngine;

namespace Features.Ui.Selection {
public class Selectable : MonoBehaviour {
    public Sprite icon;
    public string displayName;
    public GameObject uiDetailPrefab;

    private void OnDestroy() {
        Destroyed?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler Destroyed;
}
}