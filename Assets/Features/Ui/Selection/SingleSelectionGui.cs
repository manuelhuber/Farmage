using UnityEngine;
using UnityEngine.UI;

namespace Features.Ui.Selection {
public class SingleSelectionGui : MonoBehaviour {
    public Transform detailSection;
    [SerializeField] private Text displayNameText;
    [SerializeField] private Text hpText;
    [SerializeField] private Image icon;

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

    public int CurrentShield {
        get => _currentShield;
        set {
            _currentShield = value;
            UpdateHp();
        }
    }

    public int MaxShield {
        get => _maxShield;
        set {
            _maxShield = value;
            UpdateHp();
        }
    }

    public Sprite Icon {
        get => _icon;
        set {
            _icon = value;
            icon.sprite = value;
        }
    }

    private int _currentHp;
    private int _currentShield;
    private string _displayName;
    private Sprite _icon;
    private int _maxHp;
    private int _maxShield;

    private void UpdateHp() {
        var shieldText = _maxShield > 0 ? $"(+{_currentShield} / {_maxShield} shield)" : "";
        hpText.text = $"{_currentHp} / {_maxHp} {shieldText}".Trim();
    }
}
}