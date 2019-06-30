using System;
using System.Collections.Generic;
using System.Linq;
using Grimity.GameObjects;
using UnityEngine;

namespace Features.Building.Placement {
public class Placeable : MonoBehaviour {
    public Vector3 lowerCenter;
    public LayerMask terrainLayer;
    public PlacementSettings settings;
    public bool Placable => placable;

    private MeshFilter _filter;
    private MeshRenderer _renderer;
    private BoxCollider _floorChecker;
    private List<Vector3> _terrainVertices;
    private List<Vector3> _terrainVerticesWorldSpace;
    private Mesh _terrainMesh;
    private MeshCollider _terrainCollider;
    private GameObject _terrain;
    private List<Tuple<int, Vector3>> _collisionVertices = new List<Tuple<int, Vector3>>();
    private bool placable;


    private void Start() {
        _filter = GetComponent<MeshFilter>();
        _renderer = GetComponent<MeshRenderer>();
        var o = gameObject;
        _floorChecker = o.AddComponent<BoxCollider>();
        var bounds = _filter.sharedMesh.bounds;
        // TODO remove magic numbers
        _floorChecker.center = new Vector3(0, -0.5f, 0);
        _floorChecker.isTrigger = true;
        var floorCheckerSize = bounds.size;
        floorCheckerSize.y = 20;
        floorCheckerSize.x = 2;
        floorCheckerSize.z = 2;
        _floorChecker.size = floorCheckerSize;
        lowerCenter = Geometry.LowerCenter(o);
    }

    public void FlattenFloor() {
        var collisionVertices = CollisionVertices();

        var avg = collisionVertices.Select(tuple => tuple.Item2.y)
                      .Aggregate((sum, value) => sum + value) /
                  collisionVertices.Count;
        foreach (var (index, pos) in collisionVertices) {
            _terrainVertices.RemoveAt(index);
            _terrainVertices.Insert(index, new Vector3(pos.x, avg, pos.z));
        }


        var newHeight = Math.Abs(lowerCenter.y) + avg;

        transform.SetY(newHeight);
        _terrainMesh.SetVertices(_terrainVertices);
        _terrainCollider.sharedMesh = null;
        _terrainCollider.sharedMesh = _terrainMesh;
    }

    private void OnTriggerEnter(Collider other) {
        if (terrainLayer != (terrainLayer | (1 << other.gameObject.layer))) return;
        _terrainMesh = other.GetComponent<MeshFilter>().sharedMesh;
        _terrainCollider = other.GetComponent<MeshCollider>();
        _terrain = other.gameObject;
        _terrainVertices = _terrainMesh.vertices.ToList();
        var localToWorld = _terrain.transform.localToWorldMatrix;
        _terrainVerticesWorldSpace =
            _terrainVertices.Select(vector3 => localToWorld.MultiplyPoint3x4(vector3)).ToList();
    }

    private void OnTriggerStay(Collider other) {
        var collisionVertices = CollisionVertices();
        if (collisionVertices.Count == 0) return;
        var heights = collisionVertices.Select(tuple => tuple.Item2.y).ToArray();
        var dif = heights.Max() - heights.Min();
        var badTerrain = dif > settings.placementThreshold;
        if (badTerrain && placable) {
            placable = false;
            _renderer.material = settings.placementBad;
        } else if (!badTerrain && !placable) {
            placable = true;
            _renderer.material = settings.placementOk;
        }
    }

    private List<Tuple<int, Vector3>> CollisionVertices() {
        _collisionVertices.Clear();
        if (_terrainVertices == null) return _collisionVertices;
        for (var i = 0; i < _terrainVertices.Count; i++) {
            if (_floorChecker.bounds.Contains(_terrainVerticesWorldSpace[i])) {
                _collisionVertices.Add(new Tuple<int, Vector3>(i, _terrainVertices[i]));
            }
        }

        return _collisionVertices;
    }
}
}