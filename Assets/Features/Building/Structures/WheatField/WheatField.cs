using System;
using System.Collections.Generic;
using Features.Delivery;
using Features.Items;
using Features.Queue;
using Features.Resources;
using Features.Save;
using Features.Time;
using Grimity.Data;
using Ludiq.PeekCore.TinyJson;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.Building.Structures.WheatField {
public class WheatField : MonoBehaviour, ISavableComponent {
    public GameObject wheatPrefab;
    public Transform dumpingPlace;
    public int harvestCount;
    [Required] [SerializeField] private JobMultiQueue queue;
    public int growthDurationInSeconds;

    private readonly Observable<float> _progress = new Observable<float>(0);
    private GameTime _time;
    private bool _waitingForHarvest;
    private ResourceManager resourceManager;
    public Grimity.Data.IObservable<float> Progress => _progress;

    private void Awake() {
        _time = GameTime.Instance;
        resourceManager = ResourceManager.Instance;
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
        queue.Enqueue(new SimpleTask {Payload = gameObject, type = TaskType.Harvest});
    }

    public void Harvest() {
        _waitingForHarvest = false;
        _progress.Set(0);
        for (var i = 0; i < harvestCount; i++) {
            var wheat = Instantiate(wheatPrefab, dumpingPlace.position, dumpingPlace.rotation);
            queue.Enqueue(new DeliveryTask {
                Goods = wheat,
                type = TaskType.Loot,
                Target = resourceManager.GetBestStorage(wheat.GetComponent<Storable>())
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