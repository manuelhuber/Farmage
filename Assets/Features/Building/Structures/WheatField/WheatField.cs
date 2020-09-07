using System;
using System.Collections.Generic;
using Features.Delivery;
using Features.Items;
using Features.Resources;
using Features.Save;
using Features.Tasks;
using Features.Time;
using Grimity.Data;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;

namespace Features.Building.Structures.WheatField {
public class WheatField : MonoBehaviour, ISavableComponent {
    public GameObject wheatPrefab;
    public Transform dumpingPlace;
    public int harvestCount;
    public int growthDurationInSeconds;

    private readonly Observable<float> _progress = new Observable<float>(0);
    private ResourceManager _resourceManager;
    private TaskManager _taskManager;
    private GameTime _time;
    private bool _waitingForHarvest;
    public Grimity.Data.IObservable<float> Progress => _progress;

    private void Awake() {
        _time = GameTime.Instance;
        _resourceManager = ResourceManager.Instance;
        _taskManager = TaskManager.Instance;
    }

    private void Update() {
        if (_waitingForHarvest) return;
        if (_progress.Value >= growthDurationInSeconds) {
            FinishGrowth();
        } else {
            UpdateProgress();
        }
    }

    public string SaveKey => "WheatField";

    public string Save() {
        return new WheatFieldData
            {progress = _progress.Value, waitingForHarvest = _waitingForHarvest}.ToJson();
    }

    public void Load(string rawData, IReadOnlyDictionary<string, GameObject> objects) {
        var data = rawData.FromJson<WheatFieldData>();
        _progress.Set(data.progress);
        _waitingForHarvest = data.waitingForHarvest;
    }

    private void UpdateProgress() {
        _progress.Set(_progress.Value + _time.DeltaTime);
    }

    private void FinishGrowth() {
        _waitingForHarvest = true;
        _taskManager.Enqueue(new SimpleTask {Payload = gameObject, type = TaskType.Harvest});
    }

    public void Harvest() {
        _waitingForHarvest = false;
        _progress.Set(0);
        for (var i = 0; i < harvestCount; i++) {
            var wheat = Instantiate(wheatPrefab, dumpingPlace.position, dumpingPlace.rotation);
            _taskManager.Enqueue(new DeliveryTask {
                Goods = wheat,
                type = TaskType.Deliver,
                Target = _resourceManager.GetBestStorage(wheat.GetComponent<Storable>())
            });
        }
    }
}

[Serializable]
internal struct WheatFieldData {
    public float progress;
    public bool waitingForHarvest;
}
}