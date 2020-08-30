using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Features.Resources;
using Features.Time;
using Grimity.Data;
using UnityEngine;

namespace Features.Building.Production {
public class Production : MonoBehaviour {
    public int QueueSize;
    public UnitProductionEntry[] entries;
    public IObservable<ProductionOption[]> Options => _options;
    public Transform spawnPoint;

    private readonly Observable<ProductionOption[]> _options =
        new Observable<ProductionOption[]>(new ProductionOption[] { });

    public readonly Queue<UnitProductionEntry> Queue =
        new Queue<UnitProductionEntry>();

    public UnitProductionEntry Current { get; private set; }
    public float Progress { get; private set; }
    private bool _startNewProduction = true;
    private GameTime _gameTime;
    private ResourceManager _resourceManager;

    private void Awake() {
        _gameTime = GameTime.Instance;
        _resourceManager = ResourceManager.Instance;
        _resourceManager.Have.OnChange(UpdateOptions);
    }

    private void Update() {
        if (_startNewProduction) {
            if (Queue.Count == 0) return;
            Current = Queue.Dequeue();
            _startNewProduction = false;
        }

        Progress += _gameTime.DeltaTime;
        if (Progress >= Current.productionTimeInSeconds) {
            FinishProduction();
        }
    }

    private void FinishProduction() {
        Instantiate(Current.prefab, spawnPoint.position, spawnPoint.rotation);
        _startNewProduction = true;
        Progress = 0;
    }

    private void UpdateOptions(Cost _) {
        var options = entries.Select(entry => new ProductionOption {
            Image = entry.icon, Buildable = _resourceManager.CanBePayed(entry.cost),
            OnSelect = () => AddToQueue(entry)
        });
        _options.Set(options.ToArray());
    }

    private void AddToQueue(UnitProductionEntry entry) {
        if (Queue.Count >= QueueSize) return;
        if (_resourceManager.Pay(entry.cost)) {
            Queue.Enqueue(entry);
        }
    }
}
}