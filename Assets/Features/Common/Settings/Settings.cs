using Grimity.Singleton;
using UnityEngine;

namespace Features.Common.Settings {
public class Settings : GrimitySingleton<Settings> {
    [SerializeField] private Control control;
    [SerializeField] private Hotkeys hotkeys;

    public Hotkeys Hotkeys => hotkeys;

    public Control Control => control;
}
}