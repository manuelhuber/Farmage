using Grimity.Loops;
using Grimity.Mesh;
using Grimity.Rng;
using UnityEditor;
using UnityEngine;

namespace Features.Terrain {
public class FarmageTerrain : MonoBehaviour {
    public TerrainSettings settings;
    public int yMax = 5;

    private float RealWorldChunkLength => (settings.size - 1) * settings.meshScale;

    public void generateTerrain() {
        foreach (var child in GetComponentsInChildren<Transform>()) {
            if (child.gameObject == gameObject) continue;
            EditorApplication.delayCall += () => {
                if (child == null) return;
                DestroyImmediate(child.gameObject);
            };
        }

        new Loop2D(yMax).loopX((x, y) => generateChunk(new Vector2Int(x, y)));
    }

    public void generateChunk(Vector2Int pos) {
        var chunk = new GameObject {name = "Terrain", layer = 8};
        var xOffset = -(RealWorldChunkLength / 2) + RealWorldChunkLength * pos.x;
        var yOffset = -(RealWorldChunkLength / 2) + RealWorldChunkLength * pos.y;
        chunk.transform.position = new Vector3(0, 0, 0);
        chunk.transform.parent = transform;
        var meshFilter = chunk.AddComponent<MeshFilter>();
        var collider = chunk.AddComponent<MeshCollider>();
        var renderer = chunk.AddComponent<MeshRenderer>();

        chunk.AddComponent<Rigidbody>().isKinematic = true;

        var noise = Perlin.GeneratePerlinArray(settings.size,
            settings.size,
            settings.octaves,
            settings.scale,
            settings.persistence,
            settings.lacunarity,
            settings.seed,
            pos.x * (settings.size - 1),
            pos.y * (settings.size - 1));

        renderer.material = settings.material;
        var mesh = MeshGenerator.GenerateMesh(
            noise,
            settings.heightAmplifier,
            settings.heightCurve,
            scale: settings.meshScale).generateMesh();
        meshFilter.sharedMesh = mesh;
        collider.sharedMesh = mesh;
    }


    private void OnValidate() {
        if (settings == null) return;
        settings.OnValuesUpdated -= generateTerrain;
        settings.OnValuesUpdated += generateTerrain;
    }
}
}