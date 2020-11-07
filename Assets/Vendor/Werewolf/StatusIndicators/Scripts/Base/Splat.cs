using UnityEngine;
using Werewolf.StatusIndicators.Services;

namespace Werewolf.StatusIndicators.Components {
public abstract class Splat : MonoBehaviour {
    /// <summary>
    ///     Set the progress bar of Spell Indicator.
    /// </summary>
    [SerializeField] [Range(0, 1)] protected float progress;

    /// <summary>
    ///     Size of the Splat in Length, or Length and Width depending on Scaling Type
    /// </summary>
    [SerializeField] protected float scale = 7f;

    /// <summary>
    ///     Width of the Splat, when Scaling Type is Length Only
    /// </summary>
    [SerializeField] protected float width;

    // Interface

    /// <summary>
    ///     Determine if the Scaling should be uniform or length only.
    /// </summary>
    public abstract ScalingType Scaling { get; }

    // Fields

    /// <summary>
    ///     Mutable projectors of the Splat.
    /// </summary>
    public Projector[] Projectors => GetComponentsInChildren<Projector>();

    // Properties

    /// <summary>
    ///     The manager should contain all the splats for the character.
    /// </summary>
    public SplatManager Manager { get; set; }

    /// <summary>
    ///     Set the progress bar of Spell Indicator.
    /// </summary>
    public float Progress {
        get => progress;
        set {
            progress = value;
            OnValueChanged();
        }
    }

    /// <summary>
    ///     Size of the Splat in Length, or Length and Width depending on Scaling Type
    /// </summary>
    public float Scale {
        get => scale;
        set {
            scale = value;
            OnValueChanged();
        }
    }

    /// <summary>
    ///     Width of the Splat, when Scaling Type is Length Only
    /// </summary>
    public float Width {
        get => width;
        set {
            width = value;
            OnValueChanged();
        }
    }

    public virtual void Update() {
    }

    /// <summary>
    ///     We don't use Start() to avoid race conditions. Call this method from the Splat Manager.
    /// </summary>
    public virtual void Initialize() {
        // Clone all projector materials so that they don't get modified in Editor mode
        foreach (var p in Projectors) {
            p.material = new Material(p.material);
        }

        // Reset Position
        transform.localPosition = Vector3.zero;
    }

    /// <summary>
    ///     For updating the Splat whenever a value is changed.
    /// </summary>
    public virtual void OnValueChanged() {
        ProjectorScaler.Resize(Projectors, Scaling, scale, width);
        UpdateProgress(progress);
    }

    /// <summary>
    ///     Procedure when splat is set active
    /// </summary>
    public virtual void OnShow() {
    }

    /// <summary>
    ///     Cleanup procedure when set inactive
    /// </summary>
    public virtual void OnHide() {
    }

    /// <summary>
    ///     Update the progress attributes in Shader/Material.
    /// </summary>
    protected void UpdateProgress(float progress) {
        SetShaderFloat("_Fill", progress);
    }

    /// <summary>
    ///     Helper method for setting float property on all projectors/shaders for splat.
    /// </summary>
    protected void SetShaderFloat(string property, float value) {
        foreach (var p in Projectors) {
            if (p.material.HasProperty(property)) {
                p.material.SetFloat(property, value);
            }
        }
    }
}
}