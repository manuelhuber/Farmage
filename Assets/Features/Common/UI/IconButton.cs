using System;
using UnityEngine;

namespace Features.Common.UI {
public struct IconButton {
    public Sprite Icon;
    public string Hotkey;
    public Action Callback;
}
}