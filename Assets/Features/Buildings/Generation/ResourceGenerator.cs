using Features.Resources;
using Features.Tasks;
using Features.Time;
using Grimity.Data;
using UnityEngine;

namespace Features.Buildings.Generation {
public class ResourceGenerator : MonoBehaviour {
    public Resource resource;
    public Transform dumpingPlace;
    public int generationCount;
    public int generationDurationInSeconds;
    public bool autoHarvest;
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
        if (_progress.Value >= generationDurationInSeconds) {
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
        if (autoHarvest) {
            Harvest();
        } else {
            _waitingForHarvest = true;
            _harvestTask = new SimpleTask(gameObject, TaskType.Harvest);
            _taskManager.Enqueue(_harvestTask);
        }
    }

    public void Harvest() {
        _waitingForHarvest = false;
        _progress.Set(_progress.Value - generationDurationInSeconds);
        var resourceObject = resource.CreateResourceObject(generationCount);
        resourceObject.transform.position = dumpingPlace.position;
        _resourceManager.RegisterNewResource(resourceObject);
    }
}
}