using UnityEngine;

namespace Werewolf.StatusIndicators.Components {
public class LineMissile : SpellIndicator {
    public GameObject ArrowHead;
    public float MinimumRange;

    // Properties

    public override ScalingType Scaling => ScalingType.LengthOnly;
    private Projector arrowHeadProjector;

    // Fields

    private float arrowHeadScale;

    public override void Update() {
        if (Manager != null) {
            var v = FlattenVector(Manager.Get3DMousePosition()) - Manager.transform.position;
            if (v != Vector3.zero) {
                Manager.transform.rotation = Quaternion.LookRotation(v);
            }

            Scale = Mathf.Clamp((Manager.Get3DMousePosition() - Manager.transform.position).magnitude,
                MinimumRange,
                Range - ArrowHeadDistance()) * 2;
            ArrowHead.transform.localPosition = new Vector3(0, Scale * 0.5f + ArrowHeadDistance() - 0.12f, 0);
        }
    }

    // Methods

    public override void Initialize() {
        base.Initialize();
        arrowHeadProjector = ArrowHead.GetComponent<Projector>();
        arrowHeadScale = arrowHeadProjector.orthographicSize;
    }

    public override void OnValueChanged() {
        base.OnValueChanged();
        arrowHeadProjector.aspectRatio = 1f;
        arrowHeadProjector.orthographicSize = arrowHeadScale;
    }

    /// <summary>
    ///     Calculate distance of the Arrow Head from the centre point when scaling.
    /// </summary>
    private float ArrowHeadDistance() {
        return arrowHeadProjector.orthographicSize * 0.96f;
    }
}
}