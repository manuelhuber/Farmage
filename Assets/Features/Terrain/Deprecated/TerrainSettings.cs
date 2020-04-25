using Grimity.Data;
using UnityEngine;

namespace Features.Terrain {
[CreateAssetMenu()]
public class TerrainSettings : UpdatableData {
    public Material material;
    public int size = 250;
    [Range(1, 10)] public int octaves = 6;
    public float scale = 30f;
    public float meshScale = 2.5f;
    [Range(0, 1)] public float persistence = 0.4f;
    [Range(1, 10)] public float lacunarity = 2;
    public int seed = 8645153;
    public AnimationCurve heightCurve;
    public int heightAmplifier = 1;
}
}