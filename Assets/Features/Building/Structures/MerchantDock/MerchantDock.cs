using System;
using System.Linq;
using Features.Delivery;
using Features.Resources;
using Features.Tasks;
using Features.Time;
using Features.Ui.Actions;
using Grimity.Data;
using UnityEngine;

namespace Features.Building.Structures.MerchantDock {
public class MerchantDock : MonoBehaviour, IDeliveryAcceptor, IHasActions {
    public int visitIntervalsInSeconds;
    public Resource[] SellableResources;

    public bool TradeInProgress { get; private set; }

    private readonly Observable<ActionEntry[]>
        _actions = new Observable<ActionEntry[]>(new ActionEntry[0]);

    private GameTime _gameTime;
    private float _nextTradeTimestamp;
    private ResourceManager _resourceManager;
    private TaskManager _taskManager;

    private void Awake() {
        _taskManager = TaskManager.Instance;
        _gameTime = GameTime.Instance;
        _resourceManager = ResourceManager.Instance;
        _actions.Set(SellableResources.Select(CreateActionEntry).ToArray());
    }

    private void Start() {
        _nextTradeTimestamp = _gameTime.Time + visitIntervalsInSeconds;
    }

    private void Update() {
        if (TradeInProgress) return;
        if (_gameTime.Time >= _nextTradeTimestamp) {
            InitTrade();
        }
    }


    public bool AcceptDelivery(GameObject goods) {
        var resourceObject = goods.GetComponent<ResourceObject>();
        var price = resourceObject.resource.worth;
        _resourceManager.Add(price);
        Destroy(goods);
        return true;
    }

    public Grimity.Data.IObservable<ActionEntry[]> GetActions() {
        return _actions;
    }

    private ActionEntry CreateActionEntry(Resource itemType) {
        return new ActionEntry {
            Active = true,
            OnSelect = EnqueueTask(itemType)
        };
    }

    private Action EnqueueTask(Resource resource) {
        return () => {
            var item = _resourceManager.ReserveItem(resource);
            if (!item.HasValue) return;
            var (storable, storage) = item.Value;
            _taskManager.Enqueue(new DeliveryTask(
                storable.gameObject,
                storage.gameObject.AsOptional(),
                gameObject.AsOptional()
            ));
        };
    }

    private void InitTrade() {
        TradeInProgress = true;
    }

    private void FinishTrade() {
        TradeInProgress = false;
        _nextTradeTimestamp = _gameTime.Time + visitIntervalsInSeconds;
    }
}
}