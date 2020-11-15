using Features.Building.BuildMenu;
using Features.Health;
using Features.Resources;
using Features.Ui.Actions;
using Grimity.Data;
using UnityEngine;

namespace Features.Building.Construction {
public class Construction : MonoBehaviour, IHasActions {
    public float progressTarget;
    public Material material;
    public float Progress { get; private set; }

    private readonly Observable<ActionEntryData[]> _actions =
        new Observable<ActionEntryData[]>(new ActionEntryData[0]);

    private BuildingMenuEntry _building;

    public IObservable<ActionEntryData[]> GetActions() {
        return _actions;
    }

    /// <summary>
    ///     Advance the construction
    /// </summary>
    /// <param name="progress">The amount of progress that will be added</param>
    /// <returns>True if the building is now completed</returns>
    public bool Build(float progress) {
        Progress += progress;
        if (Progress < progressTarget) return false;
        Finish();
        return true;
    }

    public void Init(BuildingMenuEntry building) {
        _building = building;
        building.InitConstructionSite(gameObject);
        progressTarget = _building.buildingPrefab.GetComponent<Mortal>().MaxHitpoints;
        AddModel(_building.modelPrefab);
        _actions.Set(new[] {new ActionEntryData {Active = true, OnSelect = Cancel}});
    }

    private void AddModel(GameObject modelPrefab) {
        var model = Instantiate(modelPrefab, transform);
        foreach (var meshRenderer in model.GetComponentsInChildren<MeshRenderer>()) {
            meshRenderer.material = material;
        }
    }

    private void Cancel() {
        ResourceManager.Instance.Add(_building.cost);
        Destroy(gameObject);
    }

    private void Finish() {
        var trans = transform;
        var completedBuilding = Instantiate(_building.buildingPrefab, trans.position, trans.rotation);
        _building.InitBuilding(completedBuilding);
        Destroy(gameObject);
    }
}
}