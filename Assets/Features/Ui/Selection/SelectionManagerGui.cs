using System;
using System.Collections.Generic;
using Features.Building.UI;
using Features.Health;
using Features.Ui.Actions;
using Grimity.Data;
using UnityEngine;

namespace Features.Ui.Selection {
public class SelectionManagerGui : MonoBehaviour {
    [SerializeField] private ActionsGui actionsGui;
    private readonly List<Action> _onDeactivate = new List<Action>();

    private readonly Observable<List<ActionEntry>> currentActions =
        new Observable<List<ActionEntry>>(new List<ActionEntry>());

    private GameObject _activeUi;
    private SelectionManager _selectionManager;
    private BuildingManager buildingManager;
    private SingleSelectionGui singleSelectionGui;

    private void Awake() {
        _selectionManager = SelectionManager.Instance;
        buildingManager = BuildingManager.Instance;
        singleSelectionGui = GetComponentInChildren<SingleSelectionGui>();
        singleSelectionGui.gameObject.SetActive(false);
        currentActions.OnChange(actions => actionsGui.BuildUi(actions));
    }

    private void Start() {
        _selectionManager.Selection.OnChange(UpdateSelection);
    }

    private void UpdateSelection(List<Selectable> set) {
        ResetUi();
        switch (set.Count) {
            case 0:
                ActivateNoSelectionGui();
                return;
            case 1:
                ActivateSingleUnitGui(set[0]);
                break;
            default:
                ActivateMultipleUnitsGui(set);
                break;
        }
    }

    private void ResetUi() {
        foreach (var action in _onDeactivate) {
            action.Invoke();
        }

        _onDeactivate.Clear();
        singleSelectionGui.gameObject.SetActive(false);
        Destroy(_activeUi);
    }

    private void ActivateNoSelectionGui() {
        buildingManager.BuildingOptions.OnChange(BuildProductionUi);
        _onDeactivate.Add(() => buildingManager.BuildingOptions.RemoveOnChange(BuildProductionUi));
    }

    private void ActivateMultipleUnitsGui(List<Selectable> set) {
    }

    private void ActivateSingleUnitGui(Selectable current) {
        current.onDestroyCallbacks.Add(ResetUi);
        singleSelectionGui.gameObject.SetActive(true);
        singleSelectionGui.DisplayName = current.displayName;
        singleSelectionGui.Icon = current.icon;
        InitMortalUi(current);
        InitDetailUi(current);
        InitProductionUi(current);
    }

    private void InitProductionUi(Selectable current) {
        var production = current.GetComponent<IHasActions>();
        if (production == null) return;

        var actions = production.GetActions();
        actions.OnChange(BuildProductionUi);
        _onDeactivate.Add(() => actions.RemoveOnChange(BuildProductionUi));
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

    private void BuildProductionUi(ActionEntry[] options) {
        actionsGui.BuildUi(options);
    }
}
}