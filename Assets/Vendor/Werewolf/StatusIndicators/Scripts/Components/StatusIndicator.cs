using UnityEngine;
using Werewolf.StatusIndicators.Services;

namespace Werewolf.StatusIndicators.Components {
public class StatusIndicator : Splat {
    public int ProgressSteps;
    public override ScalingType Scaling => ScalingType.LengthAndHeight;

    public override void OnValueChanged() {
        ProjectorScaler.Resize(Projectors, Scaling, scale, width);

        if (ProgressSteps == 0) {
            UpdateProgress(progress);
        } else {
            UpdateProgress(StepProgress());
        }
    }

    /// <summary>
    ///     For a staggered fill, such as dotted circles.
    /// </summary>
    private float StepProgress() {
        var stepSize = 1.0f / ProgressSteps;
        var currentStep = Mathf.RoundToInt(progress / stepSize);
        return currentStep * stepSize - stepSize / 15;
    }
}
}