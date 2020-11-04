using System;
using System.Globalization;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Ui.Actions {
public class ActionEntry : MonoBehaviour {
    public Text cooldownText;
    public GameObject cooldownParent;
    public Image icon;
    private Button _button;

    private void Awake() {
        _button = GetComponent<Button>();
    }

    public void SetColor(Color color) {
        icon.color = color;
    }

    public void OnClick(Action action) {
        var buttonClickedEvent = new Button.ButtonClickedEvent();
        buttonClickedEvent.AddListener(action.Invoke);
        _button.onClick = buttonClickedEvent;
    }

    public void SetCooldown(float cooldown) {
        var onCooldown = cooldown > 0;
        cooldownParent.SetActive(onCooldown);
        if (!onCooldown) return;
        cooldownText.text = math.ceil(cooldown).ToString(CultureInfo.CurrentCulture);
    }
}
}