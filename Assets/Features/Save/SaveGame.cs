using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Features.Save {
public class SaveGame : MonoBehaviour {
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

    private static void LoadObjectStates(SaveData loadFile, Dictionary<string, GameObject> loadedObjects) {
        foreach (var loadFileObject in loadFile.Objects) {
            var objectData = loadFileObject.Value;
            var objectId = loadFileObject.Key;
            var loadedObject = loadedObjects[objectId];
            loadedObject.GetComponent<SavableObject>().Load(objectData, loadedObjects);
        }
    }

    private static Dictionary<string, GameObject> InstantiateSavedObjects(SaveData loadFile) {
        var readOnlyDictionary = GetSavePrefabs();
        var loadedObjects = new Dictionary<string, GameObject>();

        foreach (var loadFileObject in loadFile.Objects) {
            var objectData = loadFileObject.Value;
            var objectId = loadFileObject.Key;

            var prefabKey = objectData[SavableObject.PrefabKey];
            var prefab = readOnlyDictionary[prefabKey];
            loadedObjects[objectId] = Instantiate(prefab);
        }

        return loadedObjects;
    }

    private static Dictionary<string, GameObject> GetSavePrefabs() {
        var dict = new Dictionary<string, GameObject>();
        var savableObjects =
            GetAllPrefabs().Select(o => o.GetComponent<SavableObject>()).Where(o => o != null);
        foreach (var savableObject in savableObjects) {
            dict[savableObject.PrefabName] = savableObject.gameObject;
        }

        return dict;
    }

    private static IEnumerable<Object> GetAllPrefabs() {
        var temp = AssetDatabase.GetAllAssetPaths();
        return (from s in temp where s.Contains(".prefab") select AssetDatabase.LoadMainAssetAtPath(s))
            .ToArray();
    }
}

[Serializable]
internal struct SaveData {
    public string version;
    public Dictionary<string, Dictionary<string, string>> Data;
    public Dictionary<string, Dictionary<string, string>> Objects;
}
}