using System;
using System.Collections.Generic;
using System.Linq;
using Features.Building.BuildMenu;
using Features.Health;
using Features.Save;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;

namespace Features.Building.Construction {
public class Construction : MonoBehaviour, ISavableComponent {
    public float progressTarget;
    public Material material;
    public float Progress { get; private set; }
    private BuildingMenuEntry _building;

    public bool Build(float effort) {
        Progress += effort;
        if (Progress < progressTarget) return false;
        Finish();
        return true;
    }

    public void Init(BuildingMenuEntry building) {
        _building = building;
        building.InitConstructionSite(gameObject);
        progressTarget = _building.buildingPrefab.GetComponent<Mortal>().MaxHitpoints;
        AddModel(_building.modelPrefab);
    }

    public void AddModel(GameObject modelPrefab) {
        var model = Instantiate(modelPrefab, transform);
        foreach (var meshRenderer in model.GetComponentsInChildren<MeshRenderer>()) {
            meshRenderer.material = material;
        }
    }

    private void Finish() {
        var trans = transform;
        var completedBuilding = Instantiate(_building.buildingPrefab, trans.position, trans.rotation);
        _building.InitBuilding(completedBuilding);
        Destroy(gameObject);
    }

    public string SaveKey => "ConstructionSite";

    public string Save() {
        return new ConstructionData {progress = Progress, menuEntry = _building.buildingName}.ToJson();
    }

    public void Load(string rawData, IReadOnlyDictionary<string, GameObject> objects) {
        var data = rawData.FromJson<ConstructionData>();
        var menuEntries = SaveGame.GetAllBuildings();
        var menuEntry = menuEntries.First(entry => entry.buildingName == data.menuEntry);
        Init(menuEntry);

        Progress = data.progress;
    }
}

[Serializable]
internal struct ConstructionData {
    public float progress;
    public string menuEntry;
}
}