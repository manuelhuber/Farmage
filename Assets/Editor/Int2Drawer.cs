using JetBrains.Annotations;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[UsedImplicitly]
public class Int2Drawer : OdinValueDrawer<int2> {
    private readonly int[] _values = new int[2];
    private int2 _value;

    protected override void DrawPropertyLayout(GUIContent label) {
        _value = ValueEntry.SmartValue;

        var rect = EditorGUILayout.GetControlRect();

        // In Odin, labels are optional and can be null, so we have to account for that.
        if (label != null) {
            rect = EditorGUI.PrefixLabel(rect, label);
        }

        var labels = new GUIContent[2] {
            new GUIContent("x"),
            new GUIContent("y")
        };

        _values[0] = _value.x;
        _values[1] = _value.y;
        EditorGUI.MultiIntField(rect.AlignLeft(100f), labels, _values);
        _value.x = _values[0];
        _value.y = _values[1];

        ValueEntry.SmartValue = _value;
    }
}