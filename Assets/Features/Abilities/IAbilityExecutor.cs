namespace Features.Abilities {
public interface IAbilityExecutor {
    bool CanActivate { get; }
    void Activate();
    void Init(Ability ability);
}
}