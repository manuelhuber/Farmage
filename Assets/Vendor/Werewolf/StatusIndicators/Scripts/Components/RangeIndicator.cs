using Vendor.Werewolf.StatusIndicators.Scripts.Base;
using Vendor.Werewolf.StatusIndicators.Scripts.Options;

namespace Vendor.Werewolf.StatusIndicators.Scripts.Components {
public class RangeIndicator : Splat {
    public float DefaultScale;
    public override ScalingType Scaling => ScalingType.LengthAndHeight;

    public override void OnShow() {
        UpdateRangeIndicatorSize();
    }

    /// <summary>
    ///     Scale Range Indicator back to default
    /// </summary>
    private void UpdateRangeIndicatorSize() {
        Scale = DefaultScale;
    }
}
}