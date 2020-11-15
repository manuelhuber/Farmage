using System.Collections.Generic;
using System.Linq;
using Features.Resources;
using Features.Time;
using Features.Ui.Actions;
using Features.Ui.UserInput;
using Grimity.Data;
using UnityEngine;

namespace Features.Buildings.Production {
public class Production : MonoBehaviour, IHasActions, IOnKeyDown, IOnKeyUp {
    public int queueSize;
    public UnitProductionEntry[] entries;
    public Transform spawnPoint;
    public UnitProductionEntry Current { get; private set; }
    public float Progress { get; private set; }

    private readonly Observable<ActionEntryData[]> _options =
        new Observable<ActionEntryData[]>(new ActionEntryData[] { });

    public readonly Queue<UnitProductionEntry> Queue =
        new Queue<UnitProductionEntry>();

    private GameTime _gameTime;
    private ResourceManager _resourceManager;
    private bool _startNewProduction = true;

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

    #region InputReceiver

    public event YieldControlHandler YieldControl;

    public void OnKeyDown(HashSet<KeyCode> keys, HashSet<KeyCode> pressedKeys, MouseLocation mouseLocation) {
        if (keys.Contains(KeyCode.Mouse0)) YieldControl?.Invoke(this, new YieldControlEventArgs());
    }

    public void OnKeyUp(HashSet<KeyCode> keys, HashSet<KeyCode> pressedKeys, MouseLocation mouseLocation) {
        if (keys.Contains(KeyCode.Mouse1)) spawnPoint.transform.position = mouseLocation.Position;
    }

    #endregion

    public IObservable<ActionEntryData[]> GetActions() {
        return _options;
    }

    private void FinishProduction() {
        Instantiate(Current.prefab, spawnPoint.position, spawnPoint.rotation);
        _startNewProduction = true;
        Progress = 0;
    }

    private void UpdateOptions(Cost _) {
        var options = entries.Select(entry => new ActionEntryData {
            Image = entry.icon, Active = _resourceManager.CanBePayed(entry.cost),
            OnSelect = () => AddToQueue(entry)
        });
        _options.Set(options.ToArray());
    }

    private void AddToQueue(UnitProductionEntry entry) {
        if (Queue.Count >= queueSize) return;
        if (_resourceManager.Pay(entry.cost)) {
            Queue.Enqueue(entry);
        }
    }
}
}