using Features.Common;
using Features.Ui.Selection;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Buildings.Generation {
public class ResourceGenerationGui : MonoBehaviour, ISingleSelectionDetailGui {
    public Text progessText;
    private ResourceGenerator _generator;
    private int _growthGoal;

    private void OnDestroy() {
        _generator.Progress.RemoveOnChange(SetProgress);
    }

    public void Init(GameObject selectedUnit) {
        _generator = selectedUnit.GetComponent<ResourceGenerator>();
        _growthGoal = _generator.generationDurationInSeconds;
        _generator.Progress.OnChange(SetProgress);
    }

    private void SetProgress(float currentGrowth) {
        var percentage = TextUtil.PercentageString(currentGrowth, _growthGoal);
        progessText.text = $"Growth: {percentage}";
    }
}
}