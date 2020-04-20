namespace Features.Units.Robots {
public interface WokerbotBehaviour {
    int ActivationPriority();
    void Behave();
    void Abandon();
}
}