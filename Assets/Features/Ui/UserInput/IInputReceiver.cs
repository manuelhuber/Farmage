using System;
using System.Collections.Generic;
using UnityEngine;

namespace Features.Ui.UserInput {
public interface IInputReceiver {
    event EventHandler YieldControl;
    void OnKeyDown(HashSet<KeyCode> keys, MouseLocation mouseLocation);
    void OnKeyUp(HashSet<KeyCode> keys, MouseLocation mouseLocation);
    void OnKeyPressed(HashSet<KeyCode> keys, MouseLocation mouseLocation);
}
}