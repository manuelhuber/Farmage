using System;
using UnityEngine;

namespace Features.Ui.Actions {
public class ActionEntryData {
    public bool Active;
    public float Cooldown;
    public Sprite Image;
    public Action OnSelect;
}
}