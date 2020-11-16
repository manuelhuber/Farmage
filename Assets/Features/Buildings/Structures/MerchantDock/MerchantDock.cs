using System.Collections.Generic;
using System.Linq;
using Features.Buildings.BuildMenu;
using Features.Buildings.UI;
using Features.Merchant;
using Features.Resources;
using Features.Time;
using Features.Ui.Actions;
using Grimity.Collections;
using Grimity.Data;
using UnityEngine;

namespace Features.Buildings.Structures.MerchantDock {
public class MerchantDock : MonoBehaviour, IHasActions {
    public int visitIntervalsInSeconds;
    public int stayDurationInSeconds;
    public int itemSlots;

    [SerializeField] private MerchantInventory inventory;

    private readonly Observable<ActionEntryData[]>
        _actions = new Observable<ActionEntryData[]>(new ActionEntryData[0]);

    private readonly List<MerchantEntry> _rolledItems = new List<MerchantEntry>();
    private BuildingManager _buildingManager;

    private GameTime _gameTime;
    private float _nextTradeTimestamp;
    private ResourceManager _resourceManager;
    private bool _tradeInProgress;

    private void Awake() {
        _gameTime = GameTime.Instance;
        _resourceManager = ResourceManager.Instance;
        _buildingManager = BuildingManager.Instance;
        _resourceManager.Have.OnChange(cost => UpdateActionEntries());
    }

    private void Start() {
        _nextTradeTimestamp = _gameTime.Time + visitIntervalsInSeconds;
    }

    private void Update() {
        if (_gameTime.Time < _nextTradeTimestamp) return;
        if (_tradeInProgress) {
            FinishTrade();
        } else {
            InitTrade();
        }
    }

    public IObservable<ActionEntryData[]> GetActions() {
        return _actions;
    }

    private void FinishTrade() {
        Debug.Log("TRADE ENDED");
        _tradeInProgress = false;
        _rolledItems.Clear();
        UpdateActionEntries();
        _nextTradeTimestamp = _gameTime.Time + visitIntervalsInSeconds;
    }

    private void InitTrade() {
        Debug.Log("TRADE STARTED");
        _tradeInProgress = true;
        _nextTradeTimestamp = _gameTime.Time + stayDurationInSeconds;
        CreateActionsMenu();
        UpdateActionEntries();
    }

    private void CreateActionsMenu() {
        var lootTable = new List<MerchantEntry>();
        foreach (var entry in inventory.costs) {
            for (var i = 0; i < entry.rollWeight; i++) {
                lootTable.Add(entry);
            }
        }

        _rolledItems.Clear();
        for (var i = 0; i < itemSlots; i++) {
            _rolledItems.Add(lootTable.GetRandomElement());
        }
    }

    private void UpdateActionEntries() {
        _actions.Set(_rolledItems.Select(CreateActionEntry).ToArray());
    }

    private ActionEntryData CreateActionEntry(MerchantEntry arg) {
        if (!(arg.item is BuildingMenuEntry buildingMenuEntry)) return null;

        void BuyBuilding() {
            if (_resourceManager.Pay(arg.cost)) {
                _buildingManager.AddBuildingOption(buildingMenuEntry);
                _rolledItems.Remove(arg);
                UpdateActionEntries();
            }
        }

        return new ActionEntryData {
            Image = buildingMenuEntry.image,
            Active = _resourceManager.CanBePayed(arg.cost),
            OnSelect = BuyBuilding
        };
    }
}
}