namespace Features.Health {
public struct Damage {
    public int Amount;
    public bool IsHeal => Amount < 0;
}
}