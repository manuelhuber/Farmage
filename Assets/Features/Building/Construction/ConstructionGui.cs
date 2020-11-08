using Features.Common;
using Features.Ui.Selection;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Building.Construction {
public class ConstructionGui : MonoBehaviour, ISingleSelectionDetailGui {
    public Text progressText;
    private Construction _construction;
    private float _target;

    private void Update() {
        UpdateProgress();
    }

    public void Init(GameObject selectedUnit) {
        _construction = selectedUnit.GetComponent<Construction>();
        _target = _construction.progressTarget;
    }

    private void UpdateProgress() {
        var percentage = TextUtil.PercentageString(_construction.Progress, _target);
        progressText.text = $"Progress: {percentage}";
    }
}
}