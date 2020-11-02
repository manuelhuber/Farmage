using Features.Ui.UserInput;

namespace Features.Units.Walkers.Abilities {
public interface IAbilityExecutor : IInputReceiver {
    bool CanActivate { get; }
    void Activate();
    void Init(Ability ability);
}
}