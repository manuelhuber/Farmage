using UnityEngine;

namespace Werewolf.StatusIndicators.Components {
public class Point : SpellIndicator {
    /// <summary>
    ///     Determine if you want the Splat to be restricted to the Range Indicator bounds. Applies to "Point" Splats
    ///     only.
    /// </summary>
    [SerializeField] protected bool restrictCursorToRange;

    public override ScalingType Scaling => ScalingType.LengthAndHeight;

    public override void Update() {
        transform.position = Manager.Get3DMousePosition();
        if (restrictCursorToRange) {
            RestrictCursorToRange();
        }
    }

    private void LateUpdate() {
        // Prevent Splat from spinning due to player rotation
        transform.eulerAngles = new Vector3(90, 0, 0);
    }

    /// <summary>
    ///     Restrict splat position bound to range from player
    /// </summary>
    private void RestrictCursorToRange() {
        if (Manager != null) {
            if (Vector3.Distance(Manager.transform.position, transform.position) > range) {
                transform.position = Manager.transform.position +
                                     Vector3.ClampMagnitude(transform.position - Manager.transform.position,
                                         range);
            }
        }
    }
}
}