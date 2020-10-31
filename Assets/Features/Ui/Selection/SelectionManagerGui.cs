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

    private readonly Observable<List<ActionEntry>> _currentActions =
        new Observable<List<ActionEntry>>(new List<ActionEntry>());

    private readonly List<Action> _onDeactivate = new List<Action>();

    private GameObject _activeUi;
    private BuildingManager _buildingManager;
    private SelectionManager _selectionManager;
    private SingleSelectionGui _singleSelectionGui;

    private void Awake() {
        _selectionManager = SelectionManager.Instance;
        _buildingManager = BuildingManager.Instance;
        _singleSelectionGui = GetComponentInChildren<SingleSelectionGui>();
        _singleSelectionGui.gameObject.SetActive(false);
        _currentActions.OnChange(actions => actionsGui.BuildUi(actions));
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
        _singleSelectionGui.gameObject.SetActive(false);
        Destroy(_activeUi);
    }

    private void ActivateNoSelectionGui() {
        _buildingManager.BuildingOptions.OnChange(BuildProductionUi);
        _onDeactivate.Add(() => _buildingManager.BuildingOptions.RemoveOnChange(BuildProductionUi));
    }

    private void ActivateMultipleUnitsGui(List<Selectable> set) {
        // TODO
    }

    private void ActivateSingleUnitGui(Selectable current) {
        void OnDestroyed(object sender, EventArgs args) {
            ResetUi();
        }

        current.Destroyed += OnDestroyed;
        _onDeactivate.Add(() => current.Destroyed -= OnDestroyed);
        _singleSelectionGui.gameObject.SetActive(true);
        _singleSelectionGui.DisplayName = current.displayName;
        _singleSelectionGui.Icon = current.icon;
        InitMortalUi(current);
        InitDetailUi(current);
        InitActionUi(current);
    }

    private void InitActionUi(Selectable current) {
        var production = current.GetComponent<IHasActions>();
        if (production == null) return;

        var actions = production.GetActions();
        actions.OnChange(BuildProductionUi);
        _onDeactivate.Add(() => actions.RemoveOnChange(BuildProductionUi));
    }

    private void InitDetailUi(Selectable current) {
        if (current.uiDetailPrefab == null) return;
        _activeUi = Instantiate(current.uiDetailPrefab, _singleSelectionGui.detailSection);
        var detailUi = _activeUi.GetComponent<ISingleSelectionDetailGui>();
        detailUi?.Init(current.gameObject);
    }

    private void InitMortalUi(Selectable current) {
        var mortal = current.GetComponent<Mortal>();
        if (mortal == null) return;
        _singleSelectionGui.MaxHp = mortal.MaxHitpoints;
        mortal.Hitpoints.OnChange(SetSingleUnitCurrentHp);
        _onDeactivate.Add(() => mortal.Hitpoints.RemoveOnChange(SetSingleUnitCurrentHp));
    }

    private void SetSingleUnitCurrentHp(int i) {
        _singleSelectionGui.CurrentHp = i;
    }

    private void BuildProductionUi(ActionEntry[] options) {
        actionsGui.BuildUi(options);
    }
}
}