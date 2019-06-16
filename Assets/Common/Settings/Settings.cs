using Grimity.Singleton;
using UnityEngine;

namespace Common.Settings {
public class Settings : GrimitySingleton<Settings> {
    [SerializeField] private Hotkeys hotkeys;
    [SerializeField] private Control control;

    public Hotkeys Hotkeys => hotkeys;

    public Control Control => control;
}
}