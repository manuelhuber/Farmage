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

    public Sprite Icon {
        get => _icon;
        set {
            _icon = value;
            icon.sprite = value;
        }
    }

    private int _currentHp;
    private string _displayName;
    private Sprite _icon;
    private int _maxHp;

    private void UpdateHp() {
        hpText.text = $"{_currentHp} / {_maxHp}";
    }
}
}