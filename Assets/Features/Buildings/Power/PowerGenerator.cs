using System;
using System.Collections.Generic;
using System.Linq;
using Features.Common;
using Features.Health;
using Features.Ui.Actions;
using Features.Ui.UserInput;
using Grimity.Data;
using UnityEngine;

namespace Features.Buildings.Power {
internal enum GeneratorAction {
    Connect,
    Disconnect
}

public class PowerGenerator : MonoBehaviour, IHasActions, IKeyUpReceiver, IInputYielder {
    public int range;
    public int powerAmount;
    public Sprite connectIcon;
    public Sprite disconnectIcon;
    public int AvailablePower => powerAmount - _actualConsumers.Sum(consumer => consumer.PowerRequirements);

    private readonly Observable<ActionEntryData[]> _actions =
        new Observable<ActionEntryData[]>(new ActionEntryData[] { });

    private readonly HashSet<IPowerConsumer> _actualConsumers = new HashSet<IPowerConsumer>();
    private readonly HashSet<IPowerConsumer> _potentialConsumers = new HashSet<IPowerConsumer>();

    private GeneratorAction _currentAction;
    private InputManager _inputManager;

    private void Awake() {
        _inputManager = InputManager.Instance;
        var rangeCollider = RangeCollider.AddTo(gameObject, range);
        rangeCollider.OnEnter(AddBuilding);
        rangeCollider.OnExit(RemoveConsumer);

        var mortal = GetComponent<Mortal>();
        if (mortal != null) mortal.onDeath.AddListener(UnplugAll);

        _actions.Set(new[] {
            new ActionEntryData {Image = connectIcon, OnSelect = StartAction(GeneratorAction.Connect)},
            new ActionEntryData {Image = disconnectIcon, OnSelect = StartAction(GeneratorAction.Disconnect)}
        });
    }

    #region InputReceiver

    public void OnKeyUp(HashSet<KeyCode> keys, HashSet<KeyCode> pressedKeys, MouseLocation mouseLocation) {
        if (keys.Contains(KeyCode.Escape)) YieldControl?.Invoke(this, new YieldControlEventArgs());
        if (!keys.Contains(KeyCode.Mouse0)) return;
        var consumer = mouseLocation.Collision.GetComponent<IPowerConsumer>();
        if (consumer != null) TakeActionOn(consumer);
        YieldControl?.Invoke(this, new YieldControlEventArgs(true));
    }

    public event YieldControlHandler YieldControl;

    #endregion

    public Grimity.Data.IObservable<ActionEntryData[]> GetActions() {
        return _actions;
    }

    private Action StartAction(GeneratorAction generatorAction) {
        return () => {
            _currentAction = generatorAction;
            _inputManager.RequestControl(this);
        };
    }

    private void AddBuilding(Collider obj) {
        var consumer = obj.GetComponent<IPowerConsumer>();
        if (consumer == null) return;
        _potentialConsumers.Add(consumer);
        AddPotentialConsumers();

        var mortal = obj.GetComponent<Mortal>();
        if (mortal == null) return;
        mortal.onDeath.AddListener(() => RemoveConsumer(consumer));
    }

    private void RemoveConsumer(Collider obj) {
        var consumer = obj.GetComponent<IPowerConsumer>();
        if (consumer != null) RemoveConsumer(consumer);
    }

    private void RemoveConsumer(IPowerConsumer consumer) {
        _potentialConsumers.Remove(consumer);
        _actualConsumers.Remove(consumer);
        AddPotentialConsumers();
    }

    private void AddPotentialConsumers() {
        while (true) {
            var newConsumer = _potentialConsumers
                .FirstOrDefault(consumer => consumer.PowerRequirements <= AvailablePower);
            if (newConsumer == null) return;
            SupplyPowerTo(newConsumer);
        }
    }

    private void SupplyPowerTo(IPowerConsumer newConsumer) {
        _actualConsumers.Add(newConsumer);
        _potentialConsumers.Remove(newConsumer);
        newConsumer.SupplyPower();
    }

    private void UnplugAll() {
        foreach (var consumer in _actualConsumers) {
            consumer.CutPower();
        }

        _actualConsumers.Clear();
    }

    private void TakeActionOn(IPowerConsumer consumer) {
        var canPower = _potentialConsumers.Contains(consumer) &&
                       consumer.PowerRequirements < AvailablePower;
        switch (_currentAction) {
            case GeneratorAction.Connect when canPower:
                SupplyPowerTo(consumer);
                break;
            case GeneratorAction.Connect:
                // play sound maybe?
                break;
            case GeneratorAction.Disconnect:
                var wasConsumer = _actualConsumers.Remove(consumer);
                if (!wasConsumer) break;
                _potentialConsumers.Add(consumer);
                consumer.CutPower();
                break;
        }
    }
}
}