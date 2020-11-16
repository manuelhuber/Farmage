using Features.Resources;
using JetBrains.Annotations;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

[UsedImplicitly]
public class CostDrawer : OdinValueDrawer<Cost> {
    protected override void DrawPropertyLayout(GUIContent label) {
        var value = ValueEntry.SmartValue;

        var rect = EditorGUILayout.GetControlRect();

        // In Odin, labels are optional and can be null, so we have to account for that.
        if (label != null) {
            rect = EditorGUI.PrefixLabel(rect, label);
        }

        var prev = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 10;

        value.cash = EditorGUI.IntField(rect.AlignLeft(rect.width * 0.33f), "$", value.cash);
        value.cash = EditorGUI.IntField(rect.AlignCenter(rect.width * 0.33f), "$", value.cash);
        value.cash = EditorGUI.IntField(rect.AlignRight(rect.width * 0.33f), "$", value.cash);

        EditorGUIUtility.labelWidth = prev;

        ValueEntry.SmartValue = value;
    }
}