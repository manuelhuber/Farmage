using UnityEditor;
using Vendor.Werewolf.StatusIndicators.Scripts.Base;
using Vendor.Werewolf.StatusIndicators.Scripts.Components;

namespace Werewolf.StatusIndicators.Editors {
public class SplatEditor<T> : Editor where T : Splat {
    private T instance => (T) target;

    public override void OnInspectorGUI() {
        if (instance == null) {
            return;
        }

        EditorGUI.BeginChangeCheck();

        DrawDefaultInspector();

        if (EditorGUI.EndChangeCheck()) {
            if (instance.gameObject.scene.name != null) {
                instance.Manager = instance.Manager ?? instance.transform.parent.GetComponent<SplatManager>();
            }

            instance.OnValueChanged();
        }
    }
}

[CustomEditor(typeof(LineMissile))]
public class LineMissileEditor : SplatEditor<LineMissile> {
}

[CustomEditor(typeof(Cone))]
public class ConeEditor : SplatEditor<Cone> {
}

[CustomEditor(typeof(Point))]
public class PointEditor : SplatEditor<Point> {
}

[CustomEditor(typeof(AngleMissile))]
public class AngleMissileEditor : SplatEditor<AngleMissile> {
}

[CustomEditor(typeof(RangeIndicator))]
public class RangeIndicatorEditor : SplatEditor<RangeIndicator> {
}

[CustomEditor(typeof(StatusIndicator))]
public class StatusIndicatorEditor : SplatEditor<StatusIndicator> {
}
}