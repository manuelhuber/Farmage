using System;
using System.Collections.Generic;
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
    public Cost harvestValue;
    public Grimity.Data.IObservable<float> Progress => _progress;

    private readonly Observable<float> _progress = new Observable<float>(0);
    [Required] [SerializeField] private JobMultiQueue queue;
    private bool _waitingForHarvest;
    public int growthDurationInSeconds;
    private GameTime _time;

    private void Update() {
        if (_waitingForHarvest) return;
        if (_progress.Value >= growthDurationInSeconds)
            FinishGrowth();
        else
            UpdateProgress();
    }

    private void Awake() {
        _time = GameTime.Instance;
    }

    private void UpdateProgress() {
        _progress.Set(_progress.Value + _time.DeltaTime);
    }

    private void FinishGrowth() {
        _waitingForHarvest = true;
        queue.Enqueue(new Task {payload = gameObject, type = TaskType.Harvest});
    }

    public Cost Harvest() {
        if (!_waitingForHarvest) return new Cost();
        _waitingForHarvest = false;
        _progress.Set(0);
        return harvestValue;
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
}

[Serializable]
internal struct WheatFieldData {
    public float progress;
    public bool waitingForHarvest;
}
}