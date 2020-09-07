using System;
using Features.Building.UI;
using Features.Resources;
using Features.Save;
using Features.Ui.Selection;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Features.Building.BuildMenu {
[Serializable]
[CreateAssetMenu(menuName = "buildings/building menu entry")]
public class BuildingMenuEntry : ScriptableObject {
    [Required] public string buildingName;
    [Required] public GameObject buildingPrefab;
    [Required] public GameObject modelPrefab;
    [Required] public int2 size;
    [Required] public Cost cost;
    public Sprite image;
}

public static class BuildingMenuEntryExtensions {
    public static void InitBuilding(this BuildingMenuEntry entry, GameObject building) {
        InitSavableObject(building, entry.buildingName);
        InitUnit(building, entry);
        InitBoxCollider(building, entry);
    }

    public static void InitConstructionSite(this BuildingMenuEntry entry, GameObject building) {
        InitUnit(building, entry);
        InitBoxCollider(building, entry);
    }

    private static void InitBoxCollider(GameObject building, BuildingMenuEntry entry) {
        var boxCollider = building.GetComponent<BoxCollider>();
        boxCollider.size = new Vector3(entry.size.x, 1, entry.size.y) * BuildingManager.GridSize;
    }

    private static void InitUnit(GameObject building, BuildingMenuEntry entry) {
        var unit = building.GetComponent<Selectable>();
        unit.icon = entry.image;
        unit.displayName = entry.buildingName;
    }

    private static void InitSavableObject(GameObject building, string name) {
        var savableObject = building.GetComponent<SavableObject>();
        savableObject.PrefabName = name;
    }
}
}