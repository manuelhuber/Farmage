using Features.Buildings.Power;
using Features.Resources;
using Features.Tasks;
using Features.Time;
using Grimity.Data;
using UnityEngine;

namespace Features.Buildings.Generation {
public class ResourceGenerator : MonoBehaviour, IPowerConsumer {
    [SerializeField] private Resource resource;
    [SerializeField] private Transform dumpingPlace;
    public int generationCount;
    public int generationDurationInSeconds;
    public bool autoHarvest;
    [SerializeField] private int powerRequirements;
    public IObservable<float> Progress => _progress;

    private readonly Observable<float> _progress = new Observable<float>(0);
    private SimpleTask _harvestTask;
    private bool _hasPower;
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
        if (powerRequirements > 0 && !_hasPower) return;
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

    public int PowerRequirements => powerRequirements;

    public void SupplyPower() {
        _hasPower = true;
    }

    public void CutPower() {
        _hasPower = false;
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