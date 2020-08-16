using System;
using System.Collections.Generic;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;

namespace Features.Save {
public class SavableObject : SavableData {
    [SerializeField] public string PrefabName;
    public const string PrefabKey = "prefab";


    public override Dictionary<string, string> Save() {
        var saveData = GetSaveDataFromComponents();
        SaveTransform(saveData);
        saveData[PrefabKey] = PrefabName;
        return saveData;
    }

    public override void Load(IReadOnlyDictionary<string, string> data,
                              IReadOnlyDictionary<string, GameObject> objects) {
        LoadTransform(data);
        LoadComponents(data, objects);
    }

    private void SaveTransform(Dictionary<string, string> saveData) {
        var trans = new TransformData();
        var position = transform.position;
        var rotation = transform.rotation;
        trans.posX = position.x;
        trans.posZ = position.z;
        trans.posY = position.y;
        trans.rotX = rotation.x;
        trans.rotZ = rotation.z;
        trans.rotY = rotation.y;

        saveData["transform"] = trans.ToJson();
    }

    private void LoadTransform(IReadOnlyDictionary<string, string> data) {
        var trans = data["transform"].FromJson<TransformData>();
        var pos = new Vector3(
            trans.posX,
            trans.posY,
            trans.posZ
        );
        transform.position = pos;
    }
}

[Serializable]
public struct TransformData {
    public float posX;
    public float posZ;
    public float posY;

    public float rotX;
    public float rotZ;
    public float rotY;
}
}