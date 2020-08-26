using System;
using UnityEngine;

namespace Features.Building.Construction {
public class ConstructionSite : MonoBehaviour {
    public GameObject finalBuildingPrefab;
    public float progressTarget;
    private float _progress;

    public bool Build(float effort) {
        _progress += effort;
        if (_progress < progressTarget) return false;
        Finish();
        return true;
    }

    private void Finish() {
        var trans = transform;
        Instantiate(finalBuildingPrefab, trans.position, trans.rotation);
        Destroy(gameObject);
    }
}
}