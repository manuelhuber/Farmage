using System;
using System.Collections.Generic;
using System.Linq;
using Features.Building.BuildMenu;
using Grimity.Data;
using Grimity.Singleton;
using JetBrains.Annotations;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Features.Save {
public class SaveGame : GrimitySingleton<SaveGame> {
    public Dictionary<string, GameObject> PrefabDict { get; private set; }
    private Dictionary<string, BuildingMenuEntry> _buildingDict;

    [UsedImplicitly]
    public void Save() {
        var save = new SaveData {version = "0.1"};
        var saveData = new Dictionary<string, Dictionary<string, string>>();
        foreach (var savableData in FindObjectsOfType<SavableSingleton>()) {
            saveData[savableData.Key] = savableData.Save();
        }

        var saveObjects = new Dictionary<string, Dictionary<string, string>>();
        foreach (var savableData in FindObjectsOfType<SavableObject>()) {
            saveObjects[savableData.getSaveID()] = savableData.Save();
        }

        save.Data = saveData;
        save.Objects = saveObjects;

        new SaveFileWriter().SaveFile(save);
    }

    [UsedImplicitly]
    public void Load() {
        PrefabDict = GetSavePrefabs();
        _buildingDict = GetBuildingMenuEntries();

        var loadFile = new SaveFileWriter().LoadFile<SaveData>();
        var saveData = loadFile.Data;

        var loadedObjects = InstantiateSavedObjects(loadFile);

        foreach (var savableObject in FindObjectsOfType<SavableSingleton>()) {
            if (saveData.TryGetValue(savableObject.Key, out var objectData)) {
                savableObject.Load(objectData, loadedObjects);
            }
        }

        LoadObjectStates(loadFile, loadedObjects);
    }

    private void LoadObjectStates(SaveData loadFile, Dictionary<string, GameObject> loadedObjects) {
        foreach (var loadFileObject in loadFile.Objects) {
            var objectData = loadFileObject.Value;
            var objectId = loadFileObject.Key;
            var loadedObject = loadedObjects[objectId];
            loadedObject.GetComponent<SavableObject>().Load(objectData, loadedObjects);
        }
    }

    private Dictionary<string, GameObject> InstantiateSavedObjects(SaveData loadFile) {
        var loadedObjects = new Dictionary<string, GameObject>();

        foreach (var (objectId, objectData) in loadFile.Objects) {
            var prefabKey = objectData[SavableObject.PrefabKey];
            if (PrefabDict.ContainsKey(prefabKey)) {
                var prefab = PrefabDict[prefabKey];
                loadedObjects[objectId] = Instantiate(prefab);
            } else if (_buildingDict.ContainsKey(prefabKey)) {
                var menuEntry = _buildingDict[prefabKey];
                var building = Instantiate(menuEntry.buildingPrefab);
                menuEntry.InitBuilding(building);
                loadedObjects[objectId] = building;
            }
        }

        return loadedObjects;
    }

    private static Dictionary<string, GameObject> GetSavePrefabs() {
        var dict = new Dictionary<string, GameObject>();
        var savablePrefabs =
            GetAllPrefabs().Select(o => o.GetComponent<SavableObject>()).Where(o => o != null);
        foreach (var savableObject in savablePrefabs) {
            dict[savableObject.PrefabName] = savableObject.gameObject;
        }

        return dict;
    }

    private static Dictionary<string, BuildingMenuEntry> GetBuildingMenuEntries() {
        var dict = new Dictionary<string, BuildingMenuEntry>();
        var menuEntries = GetAllBuildings();
        foreach (var buildingMenuEntry in menuEntries) {
            dict[buildingMenuEntry.buildingName] = buildingMenuEntry;
        }

        return dict;
    }

    private static IEnumerable<Object> GetAllPrefabs() {
        var paths = AssetDatabase.GetAllAssetPaths();
        return (from path in paths
                where path.Contains(".prefab")
                select AssetDatabase.LoadMainAssetAtPath(path))
            .ToArray();
    }

    public static IEnumerable<BuildingMenuEntry> GetAllBuildings() {
        var paths = AssetDatabase.FindAssets("t:BuildingMenuEntry").Select(AssetDatabase.GUIDToAssetPath);
        return (from path in paths
                select (BuildingMenuEntry) AssetDatabase.LoadMainAssetAtPath(path))
            .ToArray();
    }

    #region Save

    [Serializable]
    private struct SaveData {
        public string version;
        public Dictionary<string, Dictionary<string, string>> Data;
        public Dictionary<string, Dictionary<string, string>> Objects;
    }

    #endregion
}
}