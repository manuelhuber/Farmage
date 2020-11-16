using Constants;
using Features.Buildings.BuildMenu;
using Features.Merchant;
using Features.Resources;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class BuildingCreator : OdinEditorWindow {
    private const string CorePath = "Assets/Features/Buildings/Structures";
    private const string MerchantEntryPath = "Assets/Features/Merchant/MenuEntries";
    private const string BasePrefabPath = "Assets/Features/Buildings/Structures/FarmerBuilding.prefab";

    [ValidateInput("ValidateBuildingName", "Can't have spaces!")] [BoxGroup("Basics")]
    public string buildingName;

    [BoxGroup("Basics")] public Sprite icon;
    [BoxGroup("Basics")] public int2 size;
    [BoxGroup("Basics")] public Cost buildCost;
    [BoxGroup("Merchant")] public Cost purchaseCost;
    [BoxGroup("Merchant")] public int rollWeight;
    private string Folder => $"{CorePath}/{buildingName}";

    [UsedImplicitly]
    private bool ValidateBuildingName(string building) {
        return building == null || !building.Contains(" ");
    }

    [MenuItem("Farmage/New Building")]
    private static void Init() {
        GetWindow<BuildingCreator>().Show();
    }


    [Button(ButtonSizes.Large)]
    public void GenerateNewBuilding() {
        AssetDatabase.CreateFolder(CorePath, buildingName);
        var modelPrefab = GenerateModelPrefab();
        var buildingPrefab = GenerateBuildingPrefab(modelPrefab);
        var menuEntry = GenerateMenuEntry(modelPrefab, buildingPrefab);
        GenerateMerchantEntry(menuEntry);
        Close();
    }

    private GameObject GenerateModelPrefab() {
        var model = new GameObject {name = $"{buildingName}_Model"};
        var modelPrefab = PrefabUtility.SaveAsPrefabAsset(model, $"{Folder}/{model.name}.prefab");
        DestroyImmediate(model);
        return modelPrefab;
    }

    private GameObject GenerateBuildingPrefab(Object modelPrefab) {
        var baseBuilding = AssetDatabase.LoadAssetAtPath<GameObject>(BasePrefabPath);
        var newBuilding = PrefabUtility.InstantiatePrefab(baseBuilding) as GameObject;
        var minimap = newBuilding.transform.GetChild(0).transform;
        var scale = minimap.localScale;
        var sizeX = Map.CellSize * size.x;
        var sizeY = Map.CellSize * size.y;
        scale.x = sizeX;
        scale.y = sizeY;
        minimap.localScale = scale;

        var boxCollider = newBuilding.GetComponent<BoxCollider>();
        boxCollider.size = new Vector3(sizeX, 1, sizeY);

        PrefabUtility.InstantiatePrefab(modelPrefab, newBuilding.transform);

        var buildingPrefab = PrefabUtility.SaveAsPrefabAsset(newBuilding,
            $"{Folder}/{buildingName}_Prefab.prefab");
        DestroyImmediate(newBuilding);
        return buildingPrefab;
    }

    private BuildingMenuEntry GenerateMenuEntry(GameObject modelPrefab,
                                                GameObject buildingPrefab) {
        var menuEntry = CreateInstance<BuildingMenuEntry>();
        menuEntry.buildingName = buildingName;
        menuEntry.modelPrefab = modelPrefab;
        menuEntry.buildingPrefab = buildingPrefab;
        menuEntry.cost = buildCost;
        menuEntry.image = icon;
        menuEntry.size = size;
        AssetDatabase.CreateAsset(menuEntry, $"{Folder}/{buildingName}_MenuEntry.asset");
        return menuEntry;
    }

    private void GenerateMerchantEntry(ScriptableObject item) {
        var merchantEntry = CreateInstance<MerchantEntry>();
        merchantEntry.item = item;
        merchantEntry.cost = purchaseCost;
        merchantEntry.rollWeight = rollWeight;
        AssetDatabase.CreateAsset(merchantEntry, $"{MerchantEntryPath}/{buildingName}_MerchantEntry.asset");
    }
}