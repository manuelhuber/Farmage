using Features.Common;
using UnityEngine;

namespace Features.Ui.Settings {
public class Settings : Manager<Settings> {
    [SerializeField] private Control control;
    [SerializeField] private Hotkeys hotkeys;

    public Hotkeys Hotkeys => hotkeys;

    public Control Control => control;
}
}