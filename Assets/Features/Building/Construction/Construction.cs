using System;
using System.Collections.Generic;
using System.Linq;
using Features.Building.BuildMenu;
using Features.Health;
using Features.Resources;
using Features.Save;
using Features.Ui.Actions;
using Grimity.Data;
using UnityEngine;

namespace Features.Building.Construction {
public class Construction : MonoBehaviour, ISavableComponent<ConstructionData>, IHasActions {
    public float progressTarget;
    public Material material;
    public float Progress { get; private set; }

    private readonly Observable<ActionEntry[]> _actions = new Observable<ActionEntry[]>(new ActionEntry[0]);
    private BuildingMenuEntry _building;

    public Grimity.Data.IObservable<ActionEntry[]> GetActions() {
        return _actions;
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

    #region Save

    public string SaveKey => "ConstructionSite";

    public ConstructionData Save() {
        return new ConstructionData {progress = Progress, menuEntry = _building.buildingName};
    }

    public void Load(ConstructionData data, IReadOnlyDictionary<string, GameObject> objects) {
        var menuEntries = SaveGame.GetAllBuildings();
        var menuEntry = menuEntries.First(entry => entry.buildingName == data.menuEntry);
        Init(menuEntry);

        Progress = data.progress;
    }

    #endregion
}

[Serializable]
public struct ConstructionData {
    public float progress;
    public string menuEntry;
}
}