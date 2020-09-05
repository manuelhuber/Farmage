using System;
using UnityEngine;

namespace Features.Ui.Actions {
public struct ActionEntry {
    public Sprite Image;
    public bool Active;
    public Action OnSelect;
}
}