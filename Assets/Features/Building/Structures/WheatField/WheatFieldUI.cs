using System;
using Features.Units.Common.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Building.Structures.WheatField {
public class WheatFieldUI : SingleUnitDetailUi {
    public Text progessText;
    private int _growthGoal;
    private WheatField _wheatField;

    public override void Init(GameObject selectedUnit) {
        _wheatField = selectedUnit.GetComponent<WheatField>();
        _growthGoal = _wheatField.growthDurationInSeconds;
        _wheatField.Progress.OnChange(SetProgress);
    }

    private void SetProgress(float currentGrowth) {
        var progressPercentage = _growthGoal < 0.1 ? 0 : currentGrowth / _growthGoal * 100;
        progessText.text = $"Growth: {progressPercentage:00}%";
    }

    private void OnDestroy() {
        _wheatField.Progress.RemoveOnChange(SetProgress);
    }
}
}