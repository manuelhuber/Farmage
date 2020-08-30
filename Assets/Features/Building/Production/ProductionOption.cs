using System;
using UnityEngine;

namespace Features.Building.Production {
public struct ProductionOption {
    public Sprite Image;
    public bool Buildable;
    public Action OnSelect;
}
}