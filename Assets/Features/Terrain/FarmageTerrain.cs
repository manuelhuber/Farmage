using System;
using Grimity.Mesh;
using Grimity.Rng;
using UnityEngine;

namespace Features.Terrain {
public class FarmageTerrain : MonoBehaviour {
    public Material material;
    public int size = 250;
    public int octaves = 6;
    public float scale = 2.5f;
    public float persistence = 0.4f;
    public int lacunarity = 2;
    public int seed = 8645153;
    public AnimationCurve heightCurve;
    public int heightAmplifier = 1;
    private GameObject _chunk;
    private MeshFilter _meshFilter;
    private MeshCollider _collider;
    private MeshRenderer _renderer;

    public void generateTerrain() {
        if (_chunk == null) {
            if (transform.childCount > 0 && transform.GetChild(0) != null) {
                _chunk = transform.GetChild(0).gameObject;
            } else {
                _chunk = new GameObject();
                _chunk.transform.parent = transform;
                _meshFilter = _chunk.AddComponent<MeshFilter>();
                _collider = _chunk.AddComponent<MeshCollider>();

                _renderer = _chunk.AddComponent<MeshRenderer>();
                _chunk.AddComponent<Rigidbody>().isKinematic = true;
            }
        }

        var noise = Perlin.GeneratePerlinArray(size,
                                               size,
                                               octaves,
                                               scale,
                                               persistence,
                                               lacunarity,
                                               seed,
                                               0,
                                               0);
        _renderer.material = material;
        var mesh = MeshGenerator.GenerateMesh(
            noise,
            heightAmplifier,
            heightCurve).generateMesh();
        _meshFilter.sharedMesh = mesh;
        _collider.sharedMesh = mesh;
    }

    private void OnValidate() {
        generateTerrain();
    }
}
}