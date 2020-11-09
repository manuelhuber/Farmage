using System;
using System.Collections.Generic;
using Features.Resources;
using Features.Save;
using Features.Tasks;
using Features.Time;
using Grimity.Data;
using UnityEngine;

namespace Features.Building.Structures.WheatField {
public class WheatField : MonoBehaviour, ISavableComponent<WheatFieldData> {
    public Resource wheatResource;
    public Transform dumpingPlace;
    public int harvestCount;
    public int growthDurationInSeconds;
    public Grimity.Data.IObservable<float> Progress => _progress;

    private readonly Observable<float> _progress = new Observable<float>(0);
    private ResourceManager _resourceManager;
    private TaskManager _taskManager;
    private GameTime _time;
    private bool _waitingForHarvest;
    private SimpleTask _harvestTask;

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

    private void UpdateProgress() {
        _progress.Set(_progress.Value + _time.DeltaTime);
    }

    private void FinishGrowth() {
        _waitingForHarvest = true;
        _harvestTask = new SimpleTask(gameObject, TaskType.Harvest);
        _taskManager.Enqueue(_harvestTask);
    }

    private void OnDestroy() {
        _taskManager.CancelTask(_harvestTask);
    }

    public void Harvest() {
        _waitingForHarvest = false;
        _progress.Set(0);
        var wheat = wheatResource.CreateResourceObject(harvestCount);
        wheat.transform.position = dumpingPlace.position;
        _resourceManager.RegisterNewResource(wheat);
    }

    #region Save

    public string SaveKey => "WheatField";

    public WheatFieldData Save() {
        return new WheatFieldData
            {progress = _progress.Value, waitingForHarvest = _waitingForHarvest};
    }

    public void Load(WheatFieldData data, IReadOnlyDictionary<string, GameObject> objects) {
        _progress.Set(data.progress);
        _waitingForHarvest = data.waitingForHarvest;
    }

    #endregion
}

[Serializable]
public struct WheatFieldData {
    public float progress;
    public bool waitingForHarvest;
}
}