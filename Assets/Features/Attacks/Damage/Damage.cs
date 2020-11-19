using UnityEngine;

namespace Features.Attacks.Damage {
public struct Damage {
    public GameObject Source;
    public int Amount;
    public bool IsHeal => Amount < 0;
}
}