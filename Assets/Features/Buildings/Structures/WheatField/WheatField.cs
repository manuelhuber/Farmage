using Features.Resources;
using Features.Tasks;
using Features.Time;
using Grimity.Data;
using UnityEngine;

namespace Features.Buildings.Structures.WheatField {
public class WheatField : MonoBehaviour {
    public Resource wheatResource;
    public Transform dumpingPlace;
    public int harvestCount;
    public int growthDurationInSeconds;
    public IObservable<float> Progress => _progress;

    private readonly Observable<float> _progress = new Observable<float>(0);
    private SimpleTask _harvestTask;
    private ResourceManager _resourceManager;
    private TaskManager _taskManager;
    private GameTime _time;
    private bool _waitingForHarvest;

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

    private void OnDestroy() {
        _taskManager.CancelTask(_harvestTask);
    }

    private void UpdateProgress() {
        _progress.Set(_progress.Value + _time.DeltaTime);
    }

    private void FinishGrowth() {
        _waitingForHarvest = true;
        _harvestTask = new SimpleTask(gameObject, TaskType.Harvest);
        _taskManager.Enqueue(_harvestTask);
    }

    public void Harvest() {
        _waitingForHarvest = false;
        _progress.Set(0);
        var wheat = wheatResource.CreateResourceObject(harvestCount);
        wheat.transform.position = dumpingPlace.position;
        _resourceManager.RegisterNewResource(wheat);
    }
}
}