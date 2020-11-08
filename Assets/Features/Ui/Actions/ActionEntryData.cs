using System;
using UnityEngine;

namespace Features.Ui.Actions {
public class ActionEntryData {
    public bool Active = true;
    public float Cooldown = 0;
    public Sprite Image;
    public Action OnSelect;
}
}