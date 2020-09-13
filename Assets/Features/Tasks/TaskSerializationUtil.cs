using System;
using System.Collections.Generic;
using Features.Delivery;
using Features.Save;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;

namespace Features.Tasks {
public static class TaskSerializationUtil {
    public static BaseTask LoadTask(string json, IReadOnlyDictionary<string, GameObject> objects) {
        var serialisedTask = json.FromJson<SerialisedTask>();
        var taskJson = serialisedTask.Data;
        var saveKey = serialisedTask.SaveKeyOfTask;

        if (saveKey == DeliveryTask.SaveKeyStatic) {
            var deliverTask = new DeliveryTask(null, null, null);
            deliverTask.LoadISavable(objects, taskJson);
            return deliverTask;
        }

        if (saveKey == SimpleTask.SaveKeyStatic) {
            var simpleTask = new SimpleTask(null, TaskType.Build);
            simpleTask.LoadISavable(objects, taskJson);
            return simpleTask;
        }

        throw new NotImplementedException("No deserializer for given task ");
    }

    public static string SerializeTask(BaseTask task) {
        var serialisedTask = task.SerialiseISavable(out var saveKey);
        var wrapper = new SerialisedTask {Data = serialisedTask, SaveKeyOfTask = saveKey};
        return wrapper.ToJson();
    }
}
}