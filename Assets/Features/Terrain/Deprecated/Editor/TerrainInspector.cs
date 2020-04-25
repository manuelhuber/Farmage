using UnityEditor;
using UnityEngine;

namespace Features.Terrain.Editor {
[CustomEditor(typeof(FarmageTerrain))]
public class TerrainInspector : UnityEditor.Editor {
    public override void OnInspectorGUI() {
        var generator = (FarmageTerrain) target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate")) {
            generator.generateTerrain();
        }
    }
}
}