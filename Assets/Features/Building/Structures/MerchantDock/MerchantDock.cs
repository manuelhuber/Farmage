using System;
using Features.Delivery;
using Features.Items;
using Features.Resources;
using Features.Tasks;
using Features.Time;
using Features.Ui.Actions;
using Grimity.Data;
using UnityEngine;

namespace Features.Building.Structures.MerchantDock {
public class MerchantDock : MonoBehaviour, IDeliveryAcceptor, IHasActions {
    public int visitIntervalsInSeconds;
    public SalesPrices prices;

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
        _actions.Set(new[] {
            new ActionEntry {
                Active = true,
                OnSelect = EnqueueTask(ItemType.Wheat)
            }
        });
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
        var itemType = goods.GetComponent<Storable>().type;
        if (!prices.Prices.ContainsKey(itemType)) {
            return false;
        }

        _resourceManager.Add(prices.Prices[itemType]);
        Destroy(goods);

        return true;
    }

    public Grimity.Data.IObservable<ActionEntry[]> GetActions() {
        return _actions;
    }

    private Action EnqueueTask(ItemType itemType) {
        return () => {
            var item = _resourceManager.FindItem(itemType);
            if (!item.HasValue) return;
            var (storable, storage) = item.Value;
            _taskManager.Enqueue(new DeliveryTask {
                type = TaskType.Deliver, Target = gameObject,
                Goods = storable.gameObject,
                From = storage.gameObject
            });
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