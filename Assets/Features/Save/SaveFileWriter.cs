using System.IO;
using Ludiq.PeekCore.TinyJson;
using Newtonsoft.Json;
using UnityEngine;

namespace Features.Save {
public class SaveFileWriter {
    public void SaveFile<T>(T data) {
        string destination = Application.persistentDataPath + "/save.dat";

        //
        // var bf = new BinaryFormatter();
        // bf.Serialize(file, data);
        var json = data.ToJson();
        File.WriteAllText(destination, json);
    }

    public T LoadFile<T>() {
        var destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else {
            Debug.LogError("File not found");
            return default(T);
        }

        var json = File.ReadAllText(destination);
        return json.FromJson<T>();
        // var bf = new BinaryFormatter();
        // var data = (T) bf.Deserialize(file);
        // file.Close();
    }
}
}