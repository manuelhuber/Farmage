using System;
using System.Collections.Generic;
using Features.Building.Production;
using Features.Health;
using UnityEngine;

namespace Features.Ui.Selection {
public class SelectionManagerGui : MonoBehaviour {
    [SerializeField] private ProductionGui productionGui;
    private readonly List<Action> _onDeactivate = new List<Action>();
    private GameObject _activeUi;
    private SelectionManager _selectionManager;
    private SingleSelectionGui singleSelectionGui;

    private void Awake() {
        _selectionManager = SelectionManager.Instance;
        singleSelectionGui = GetComponentInChildren<SingleSelectionGui>();
        singleSelectionGui.gameObject.SetActive(false);
    }

    private void Start() {
        _selectionManager.Selection.OnChange(UpdateSelection);
    }

    private void UpdateSelection(List<Selectable> set) {
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
        singleSelectionGui.gameObject.SetActive(false);
        Destroy(_activeUi);
        productionGui.ShowDefault();
    }

    private void ActivateMultipleUnitsGui(List<Selectable> set) {
    }

    private void ActivateSingleUnitGui(Selectable current) {
        singleSelectionGui.gameObject.SetActive(true);
        singleSelectionGui.DisplayName = current.displayName;
        singleSelectionGui.Icon = current.icon;
        InitMortalUi(current);
        InitDetailUi(current);
        InitProductionUi(current);
    }

    private void InitProductionUi(Selectable current) {
        var production = current.GetComponent<Production>();
        if (production == null) return;

        void BuildProductionUi(ProductionOption[] options) {
            productionGui.BuildUi(options);
        }

        production.Options.OnChange(BuildProductionUi);
        _onDeactivate.Add(() => production.Options.RemoveOnChange(BuildProductionUi));
    }

    private void InitDetailUi(Selectable current) {
        if (current.uiDetailPrefab == null) return;
        _activeUi = Instantiate(current.uiDetailPrefab, singleSelectionGui.detailSection);
        var detailUi = _activeUi.GetComponent<ISingleSelectionDetailGui>();
        detailUi?.Init(current.gameObject);
    }

    private void InitMortalUi(Selectable current) {
        var mortal = current.GetComponent<Mortal>();
        if (mortal == null) return;
        singleSelectionGui.MaxHp = mortal.MaxHitpoints;
        mortal.Hitpoints.OnChange(SetSingleUnitCurrentHp);
        _onDeactivate.Add(() => mortal.Hitpoints.RemoveOnChange(SetSingleUnitCurrentHp));
    }

    private void SetSingleUnitCurrentHp(int i) {
        singleSelectionGui.CurrentHp = i;
    }
}
}