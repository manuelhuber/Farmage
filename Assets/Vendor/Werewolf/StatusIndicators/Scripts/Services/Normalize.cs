using UnityEngine;

namespace Vendor.Werewolf.StatusIndicators.Scripts.Services {
public class Normalize {
    public float Factor;
    public float Max;
    public float Portion;
    public float Value;

    public Normalize(float portion, float max) {
        Portion = portion;
        Max = max;
        Factor = Portion / Max;
        Value = Mathf.Clamp(Factor, 0, 1f);
    }

    public static float GetValue(float portion, float max) {
        return Mathf.Clamp(portion / max, 0, 1f);
    }
}
}