using System.Collections.Generic;
using UnityEngine;

namespace Features.Ui.UserInput {
public interface IOnReceiveControl : IInputReceiver {
    void OnReceiveControl();
}

public interface IOnKeyDown : IInputReceiver {
    void OnKeyDown(HashSet<KeyCode> keys, HashSet<KeyCode> pressedKeys, MouseLocation mouseLocation);
}

public interface IOnKeyPressed : IInputReceiver {
    void OnKeyPressed(HashSet<KeyCode> keys, MouseLocation mouseLocation);
}

public interface IOnKeyUp : IInputReceiver {
    void OnKeyUp(HashSet<KeyCode> keys, HashSet<KeyCode> pressedKeys, MouseLocation mouseLocation);
}

public interface IInputReceiver {
    event YieldControlHandler YieldControl;
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