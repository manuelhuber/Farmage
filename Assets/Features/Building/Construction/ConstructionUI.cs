using Features.Units.Common.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Building.Construction {
public class ConstructionUI : SingleUnitDetailUi {
    public Text progressText;
    private float _target;
    private Construction _construction;

    public override void Init(GameObject selectedUnit) {
        _construction = selectedUnit.GetComponent<Construction>();
        _target = _construction.progressTarget;
    }

    private void Update() {
        UpdateProgress();
    }

    private void UpdateProgress() {
        var progressPercentage = _target < 0.1 ? 0 : _construction.Progress / _target * 100;
        progressText.text = $"Progress: {progressPercentage:00}%";
    }
}
}