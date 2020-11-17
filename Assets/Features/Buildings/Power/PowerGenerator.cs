using System.Collections.Generic;
using System.Linq;
using Features.Common;
using Features.Health;
using UnityEngine;

namespace Features.Buildings.Power {
public class PowerGenerator : MonoBehaviour {
    public int range;
    public int powerAmount;
    private readonly List<IPowerConsumer> _actualConsumers = new List<IPowerConsumer>();

    private readonly List<IPowerConsumer> _potentialConsumers = new List<IPowerConsumer>();

    private void Awake() {
        var rangeCollider = RangeCollider.AddTo(gameObject, range);
        rangeCollider.OnEnter(AddBuilding);
        rangeCollider.OnExit(RemoveBuilding);
        var mortal = GetComponent<Mortal>();
        if (mortal != null) {
            mortal.onDeath.AddListener(UnplugAll);
        }
    }

    private void AddBuilding(Collider obj) {
        var consumer = obj.GetComponent<IPowerConsumer>();
        if (consumer == null) return;
        _potentialConsumers.Add(consumer);
        CheckPotentialConsumers();

        var mortal = obj.GetComponent<Mortal>();
        if (mortal == null) return;
        mortal.onDeath.AddListener(() => RemoveConsumer(consumer));
    }

    private void RemoveBuilding(Collider obj) {
        var consumer = obj.GetComponent<IPowerConsumer>();
        if (consumer == null) return;
        RemoveConsumer(consumer);
    }

    private void RemoveConsumer(IPowerConsumer consumer) {
        _potentialConsumers.Remove(consumer);
        _actualConsumers.Remove(consumer);
        CheckPotentialConsumers();
    }

    private void CheckPotentialConsumers() {
        while (true) {
            var spentPower = _actualConsumers.Sum(powerConsumer => powerConsumer.PowerRequirements);
            var newConsumer = _potentialConsumers.FirstOrDefault(consumer =>
                spentPower + consumer.PowerRequirements <= powerAmount);

            if (newConsumer == null) return;
            _actualConsumers.Add(newConsumer);
            _potentialConsumers.Remove(newConsumer);
            newConsumer.SupplyPower();
        }
    }

    private void UnplugAll() {
        foreach (var consumer in _actualConsumers) {
            consumer.CutPower();
        }

        _actualConsumers.Clear();
    }
}
}