using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Features.Units.Common {
public class SingleUnitGui : MonoBehaviour {
    [SerializeField] private Text displayNameText;
    [SerializeField] private Text hpText;
    [SerializeField] private Image icon;

    private string _displayName;
    private int _currentHp;
    private int _maxHp;

    public string DisplayName {
        get => _displayName;
        set {
            _displayName = value;
            displayNameText.text = value;
        }
    }

    public int MaxHp {
        get => _maxHp;
        set {
            _maxHp = value;
            UpdateHp();
        }
    }

    public int CurrentHp {
        get => _currentHp;
        set {
            _currentHp = value;
            UpdateHp();
        }
    }

    private Sprite _icon;

    public Sprite Icon {
        get => _icon;
        set {
            _icon = value;
            icon.sprite = value;
        }
    }


    private void UpdateHp() {
        hpText.text = $"{_currentHp} / {_maxHp}";
    }
}
}