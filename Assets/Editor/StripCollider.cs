using UnityEditor;
using UnityEngine;

public static class StripCollider {
    [MenuItem("GameObject/Farmage/Strip Collider from children")]
    private static void StripColliderFromChildren() {
        var selected = Selection.activeObject;
        if (!(selected is GameObject gameObject)) return;
        var collider = gameObject.GetComponentsInChildren<Collider>(true);
        foreach (var col in collider) {
            Object.DestroyImmediate(col);
        }

        EditorUtility.SetDirty(selected);
    }
}