using UnityEngine;

namespace Features.Health {
public struct Damage {
    public GameObject Source;
    public int Amount;
    public bool IsHeal => Amount < 0;
}
}