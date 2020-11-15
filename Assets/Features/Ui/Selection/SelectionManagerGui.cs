using System;
using System.Collections.Generic;
using System.Linq;
using Features.Buildings.UI;
using Features.Health;
using Features.Ui.Actions;
using Grimity.Data;
using UnityEngine;

namespace Features.Ui.Selection {
public class SelectionManagerGui : MonoBehaviour {
    [SerializeField] private ActionsGui actionsGui;

    private readonly Observable<List<ActionEntryData>> _currentActions =
        new Observable<List<ActionEntryData>>(new List<ActionEntryData>());

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
        _buildingManager.BuildingOptions.OnChange(BuildActionUi);
        _onDeactivate.Add(() => _buildingManager.BuildingOptions.RemoveOnChange(BuildActionUi));
    }

    private void ActivateMultipleUnitsGui(List<Selectable> _) {
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
        var allActionComponents = current.GetComponents<IHasActions>();
        if (allActionComponents.Length == 0) return;

        void SetAllActions(ActionEntryData[] _) {
            var allActions = allActionComponents.SelectMany(hasActions => hasActions.GetActions().Value)
                .ToArray();
            BuildActionUi(allActions);
        }

        foreach (var hasActions in allActionComponents) {
            hasActions.GetActions().OnChange(SetAllActions);
            _onDeactivate.Add(() => hasActions.GetActions().RemoveOnChange(SetAllActions));
        }
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
        mortal.Shield.OnChange(SetSingleUnitCurrentShield);
        mortal.MaxShield.OnChange(SetSingleUnitMaxShield);
        _onDeactivate.Add(() => {
            mortal.Hitpoints.RemoveOnChange(SetSingleUnitCurrentHp);
            mortal.Shield.RemoveOnChange(SetSingleUnitCurrentShield);
            mortal.MaxShield.RemoveOnChange(SetSingleUnitMaxShield);
        });
    }

    private void SetSingleUnitCurrentHp(int i) {
        _singleSelectionGui.CurrentHp = i;
    }

    private void SetSingleUnitCurrentShield(int i) {
        _singleSelectionGui.CurrentShield = i;
    }

    private void SetSingleUnitMaxShield(int i) {
        _singleSelectionGui.MaxShield = i;
    }

    private void BuildActionUi(ActionEntryData[] options) {
        actionsGui.BuildUi(options);
    }
}
}