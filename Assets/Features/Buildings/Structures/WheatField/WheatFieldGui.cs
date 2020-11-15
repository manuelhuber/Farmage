using Features.Common;
using Features.Ui.Selection;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Buildings.Structures.WheatField {
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
        var percentage = TextUtil.PercentageString(currentGrowth, _growthGoal);
        progessText.text = $"Growth: {percentage}%";
    }
}
}