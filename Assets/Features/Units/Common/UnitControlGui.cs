using System;
using System.Collections.Generic;
using Features.Health;
using Features.Units.Common.Ui;
using UnityEngine;

namespace Features.Units.Common {
public class UnitControlGui : MonoBehaviour {
    private UnitControl _control;
    private SingleUnitGui _singleUnitGui;
    private GameObject _activeUi;
    private readonly List<Action> _onDeactivate = new List<Action>();

    private void Awake() {
        _control = UnitControl.Instance;
        _singleUnitGui = GetComponentInChildren<SingleUnitGui>();
        _singleUnitGui.gameObject.SetActive(false);
    }

    private void Start() {
        _control.Selection.OnChange(UpdateSelection);
    }

    private void UpdateSelection(List<Unit> set) {
        DeactivateAllUi();
        switch (set.Count) {
            case 0:
                return;
            case 1:
                ActivateSingleUnitGui(set[0]);
                break;
            default:
                ActivateMultipleUnitsGui(set);
                break;
        }
    }

    private void DeactivateAllUi() {
        foreach (var action in _onDeactivate) {
            action.Invoke();
        }

        _onDeactivate.Clear();
        _singleUnitGui.gameObject.SetActive(false);
        Destroy(_activeUi);
    }

    private void ActivateMultipleUnitsGui(List<Unit> set) {
    }

    private void ActivateSingleUnitGui(Unit current) {
        _singleUnitGui.gameObject.SetActive(true);
        _singleUnitGui.DisplayName = current.displayName;
        _singleUnitGui.Icon = current.icon;
        InitMortalUi(current);
        InitDetailUi(current);
    }

    private void InitDetailUi(Unit current) {
        if (current.uiDetailPrefab == null) return;
        _activeUi = Instantiate(current.uiDetailPrefab, _singleUnitGui.detailSection);
        var detailUi = _activeUi.GetComponent<SingleUnitDetailUi>();
        if (detailUi != null) {
            detailUi.Init(current.gameObject);
        }
    }

    private void InitMortalUi(Unit current) {
        var mortal = current.GetComponent<Mortal>();
        if (mortal == null) return;
        _singleUnitGui.MaxHp = mortal.MaxHitpoints;
        mortal.Hitpoints.OnChange(SetSingleUnitCurrentHp);
        _onDeactivate.Add(() => mortal.Hitpoints.RemoveOnChange(SetSingleUnitCurrentHp));
    }

    private void SetSingleUnitCurrentHp(int i) {
        _singleUnitGui.CurrentHp = i;
    }
}
}