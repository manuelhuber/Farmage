using System;
using System.Collections.Generic;
using UnityEngine;

namespace Features.Ui.Selection {
public class Selectable : MonoBehaviour {
    public Sprite icon;
    public string displayName;
    public GameObject uiDetailPrefab;

    public readonly List<Action> onDestroyCallbacks = new List<Action>();

    private void Start() {
        SelectionManager.Instance.Register(this);
    }

    private void OnDestroy() {
        foreach (var onDestroyCallback in onDestroyCallbacks) {
            onDestroyCallback();
        }
    }
}
}