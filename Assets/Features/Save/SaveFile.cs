using System.IO;
using UnityEngine;
using Vendor.TinyJson;

namespace Features.Save {
public static class SaveFile {
    public static void Write<T>(T data, string path) {
        var destination = Application.persistentDataPath + path;
        var json = data.ToJson();
        var file = new FileInfo(destination);
        file.Directory?.Create();
        File.WriteAllText(file.FullName, json);
    }

    public static T Load<T>(string path) {
        var destination = Application.persistentDataPath + path;

        if (!File.Exists(destination)) {
            Debug.LogError("File not found");
            return default;
        }

        var json = File.ReadAllText(destination);
        return json.FromJson<T>();
    }
}
}