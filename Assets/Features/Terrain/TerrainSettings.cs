using Grimity.Data;
using UnityEngine;

namespace Features.Terrain {
[CreateAssetMenu]
public class TerrainSettings : UpdatableData {
    public int heightAmplifier = 1;
    public AnimationCurve heightCurve;
    [Range(1, 10)] public float lacunarity = 2;
    public Material material;
    public float meshScale = 2.5f;
    [Range(1, 10)] public int octaves = 6;
    [Range(0, 1)] public float persistence = 0.4f;
    public float scale = 30f;
    public int seed = 8645153;
    public int size = 250;
}
}