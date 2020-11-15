using UnityEngine;
using Vendor.Werewolf.StatusIndicators.Scripts.Base;
using Vendor.Werewolf.StatusIndicators.Scripts.Options;

namespace Vendor.Werewolf.StatusIndicators.Scripts.Components {
public class AngleMissile : SpellIndicator {
    // Properties

    public override ScalingType Scaling => ScalingType.LengthAndHeight;

    // Methods

    public override void Update() {
        if (Manager != null) {
            var v = FlattenVector(Manager.Get3DMousePosition()) - Manager.transform.position;
            if (v != Vector3.zero) {
                Manager.transform.rotation = Quaternion.LookRotation(v);
            }
        }
    }
}
}