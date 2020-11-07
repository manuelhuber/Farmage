using System.Collections;
using UnityEngine;
using Werewolf.StatusIndicators.Services;

namespace Werewolf.StatusIndicators.Components {
public class Cone : SpellIndicator {
    // Constants

    public const float CONE_ANIM_SPEED = 0.15f;

    // Fields

    public Projector LBorder, RBorder;

    [SerializeField] [Range(0, 360)] private float angle;

    // Properties

    public override ScalingType Scaling => ScalingType.LengthAndHeight;

    public float Angle {
        get => angle;
        set {
            angle = value;
            SetAngle(value);
        }
    }

    // Methods

    public override void Update() {
        if (Manager != null) {
            Manager.transform.rotation =
                Quaternion.LookRotation(FlattenVector(Manager.Get3DMousePosition()) -
                                        Manager.transform.position);
        }
    }

    public override void OnValueChanged() {
        base.OnValueChanged();
        SetAngle(angle);
    }

    public override void OnShow() {
        base.OnShow();
        StartCoroutine(FadeIn());
    }

    private void SetAngle(float angle) {
        SetShaderFloat("_Expand", Normalize.GetValue(angle - 1, 360));
        LBorder.transform.localEulerAngles = new Vector3(0, 0, (angle + 2) / 2);
        RBorder.transform.localEulerAngles = new Vector3(0, 0, (-angle + 2) / 2);
    }

    /// <summary>
    ///     Optional animation when Cone is made visible.
    /// </summary>
    private IEnumerator FadeIn() {
        var final = angle;
        float current = 0;

        foreach (var p in Projectors) {
            p.enabled = true;
        }

        while (current < final) {
            SetAngle(current);
            current += final * CONE_ANIM_SPEED;
            yield return null;
        }

        SetAngle(final);
        yield return null;
    }
}
}