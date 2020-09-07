using System;
using System.Collections.Generic;
using System.Linq;
using Features.Building.BuildMenu;
using Features.Health;
using Features.Resources;
using Features.Save;
using Features.Ui.Actions;
using Grimity.Data;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;

namespace Features.Building.Construction {
public class Construction : MonoBehaviour, ISavableComponent, IHasActions {
    public float progressTarget;
    public Material material;
    private readonly Observable<ActionEntry[]> _actions = new Observable<ActionEntry[]>(new ActionEntry[0]);
    private BuildingMenuEntry _building;
    public float Progress { get; private set; }

    public Grimity.Data.IObservable<ActionEntry[]> GetActions() {
        return _actions;
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
        _actions.Set(new[] {
            new ActionEntry {
                Active = true,
                OnSelect = Cancel
            }
        });
    }

    private void AddModel(GameObject modelPrefab) {
        var model = Instantiate(modelPrefab, transform);
        foreach (var meshRenderer in model.GetComponentsInChildren<MeshRenderer>()) {
            meshRenderer.material = material;
        }
    }

    private void Cancel() {
        ResourceManager.Instance.Add(_building.cost);
        Destroy(this);
    }

    private void Finish() {
        var trans = transform;
        var completedBuilding = Instantiate(_building.buildingPrefab, trans.position, trans.rotation);
        _building.InitBuilding(completedBuilding);
        Destroy(gameObject);
    }
}

[Serializable]
internal struct ConstructionData {
    public float progress;
    public string menuEntry;
}
}