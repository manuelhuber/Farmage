using System.Collections.Generic;
using UnityEngine;

namespace Features.Ui.UserInput {
public interface IControlReceiver : IInputReceiver {
    void OnReceiveControl();
}

public interface IKeyDownReceiver : IInputReceiver {
    void OnKeyDown(HashSet<KeyCode> keys, HashSet<KeyCode> pressedKeys, MouseLocation mouseLocation);
}

public interface IKeyPressedReceiver : IInputReceiver {
    void OnKeyPressed(HashSet<KeyCode> keys, MouseLocation mouseLocation);
}

public interface IKeyUpReceiver : IInputReceiver {
    void OnKeyUp(HashSet<KeyCode> keys, HashSet<KeyCode> pressedKeys, MouseLocation mouseLocation);
}

public interface IInputYielder : IInputReceiver {
    event YieldControlHandler YieldControl;
}

public interface IInputReceiver {
}

public delegate void YieldControlHandler(IInputReceiver sender, YieldControlEventArgs args);

public class YieldControlEventArgs {
    public YieldControlEventArgs() : this(false) {
    }

    public YieldControlEventArgs(bool consumedKeyEvent) {
        ConsumedKeyEvent = consumedKeyEvent;
    }

    /// <summary>
    ///     True if the input receiver used the event for some action and wants to yield control after the
    ///     action
    ///     False means the input receiver didn't know what to do with the event and wants to yield control
    ///     and give someone else the opportunity to use it
    /// </summary>
    public bool ConsumedKeyEvent { get; }
}
}