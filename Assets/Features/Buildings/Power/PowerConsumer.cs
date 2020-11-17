namespace Features.Buildings.Power {
public interface IPowerConsumer {
    int PowerRequirements { get; }
    void SupplyPower();
    void CutPower();
}
}