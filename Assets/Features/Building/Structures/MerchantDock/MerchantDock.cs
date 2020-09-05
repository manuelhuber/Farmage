using System;
using Features.Delivery;
using Features.Items;
using Features.Queue;
using Features.Resources;
using Features.Time;
using Features.Ui.Actions;
using Grimity.Data;
using UnityEngine;

namespace Features.Building.Structures.MerchantDock {
public class MerchantDock : MonoBehaviour, IDeliveryAcceptor, IHasActions {
    public int visitIntervalsInSeconds;
    public SalesPrices prices;
    public JobMultiQueue queue;

    private readonly Observable<ActionEntry[]>
        actions = new Observable<ActionEntry[]>(new ActionEntry[0]);

    private GameTime gameTime;
    private float nextTradeTimestamp;
    private ResourceManager resourceManager;
    public bool TradeInProgress { get; private set; }

    private void Awake() {
        gameTime = GameTime.Instance;
        resourceManager = ResourceManager.Instance;
        actions.Set(new[] {
            new ActionEntry {
                Active = true,
                OnSelect = EnqueueTask(ItemType.Wheat)
            }
        });
    }

    private void Start() {
        nextTradeTimestamp = gameTime.Time + visitIntervalsInSeconds;
    }

    private void Update() {
        if (TradeInProgress) return;
        if (gameTime.Time >= nextTradeTimestamp) {
            InitTrade();
        }
    }

    public bool AcceptDelivery(GameObject goods) {
        var itemType = goods.GetComponent<Storable>().type;
        if (!prices.Prices.ContainsKey(itemType)) {
            return false;
        }

        resourceManager.Add(prices.Prices[itemType]);
        Destroy(goods);

        return true;
    }

    public Grimity.Data.IObservable<ActionEntry[]> GetActions() {
        return actions;
    }

    private Action EnqueueTask(ItemType itemType) {
        return () => {
            var item = resourceManager.FindItem(itemType);
            if (!item.HasValue) return;
            var (storable, storage) = item.Value;
            queue.Enqueue(new DeliveryTask {
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
        nextTradeTimestamp = gameTime.Time + visitIntervalsInSeconds;
    }
}
}