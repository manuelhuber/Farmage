﻿using UnityEngine;
using Vendor.Werewolf.StatusIndicators.Scripts.Options;

namespace Vendor.Werewolf.StatusIndicators.Scripts.Services {
public class ProjectorScaler {
    public static void Resize(Projector projector, float scale) {
        if (projector != null) {
            projector.orthographicSize = scale / 2;
        }
    }

    public static void Resize(Projector projector, ScalingType scaling, float scale, float width) {
        if (projector != null) {
            if (scaling != ScalingType.None) {
                if (scaling == ScalingType.LengthOnly) {
                    projector.aspectRatio = width / scale;
                } else {
                    projector.aspectRatio = 1f;
                }

                projector.orthographicSize = scale / 2;
            }
        }
    }

    public static void Resize(Projector[] projectors, ScalingType scaling, float scale, float width) {
        foreach (var p in projectors) {
            Resize(p, scaling, scale, width);
        }
    }

    public static void Resize(Projector[] projectors, float scale) {
        foreach (var p in projectors) {
            Resize(p, scale);
        }
    }
}
}