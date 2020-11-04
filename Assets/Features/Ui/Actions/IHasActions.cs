using Grimity.Data;

namespace Features.Ui.Actions {
public interface IHasActions {
    IObservable<ActionEntryData[]> GetActions();
}
}