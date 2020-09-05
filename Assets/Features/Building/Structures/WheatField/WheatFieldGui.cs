using Features.Ui.Selection;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Building.Structures.WheatField {
public class WheatFieldGui : MonoBehaviour, ISingleSelectionDetailGui {
    public Text progessText;
    private int _growthGoal;
    private WheatField _wheatField;

    private void OnDestroy() {
        _wheatField.Progress.RemoveOnChange(SetProgress);
    }

    public void Init(GameObject selectedUnit) {
        _wheatField = selectedUnit.GetComponent<WheatField>();
        _growthGoal = _wheatField.growthDurationInSeconds;
        _wheatField.Progress.OnChange(SetProgress);
    }

    private void SetProgress(float currentGrowth) {
        var progressPercentage = _growthGoal < 0.1 ? 0 : currentGrowth / _growthGoal * 100;
        progessText.text = $"Growth: {progressPercentage:00}%";
    }
}
}