using System.Collections.Generic;
using Features.Health;
using UnityEngine;

namespace Features.Units.Common {
public class UnitControlGui : MonoBehaviour {
    public SingleUnitGui _SingleUnitGuiPrefab;
    private UnitControl _control;
    private SingleUnitGui _singleUnitGui;

    private void Awake() {
        _control = UnitControl.Instance;
        // _singleUnitGui = Instantiate(_SingleUnitGuiPrefab, transform).GetComponent<SingleUnitGui>();
        _singleUnitGui = GetComponentInChildren<SingleUnitGui>();
        _singleUnitGui.gameObject.SetActive(false);
    }

    private void Start() {
        _control.Selection.OnChange(UpdateSelection);
    }

    private void UpdateSelection(List<Unit> set) {
        switch (set.Count) {
            case 0:
                DeactivateAllUi();
                break;
            case 1:
                ActivateSingleUnitGui(set[0]);
                break;
            default:
                ActivateMultipleUnitsGui(set);
                break;
        }
    }

    private void DeactivateAllUi() {
        _singleUnitGui.gameObject.SetActive(false);
    }

    private void ActivateMultipleUnitsGui(List<Unit> set) {
    }

    private void ActivateSingleUnitGui(Unit current) {
        _singleUnitGui.gameObject.SetActive(true);
        _singleUnitGui.DisplayName = current.displayName;
        var mortal = current.GetComponent<Mortal>();
        if (mortal == null) return;
        _singleUnitGui.MaxHp = mortal.MaxHitpoints;
        mortal.Hitpoints.OnChange(i => _singleUnitGui.CurrentHp = i);
    }
}
}