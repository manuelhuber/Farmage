using Features.Delivery;
using Features.Resources;
using Features.Tasks;
using Grimity.Data;
using JetBrains.Annotations;
using UnityEngine;

namespace Features.Items.Loot {
public class LootDrop : MonoBehaviour {
    public LootTable lootTable;
    private ResourceManager _resourceManager;
    private TaskManager _taskManager;

    private void Awake() {
        _resourceManager = ResourceManager.Instance;
        _taskManager = TaskManager.Instance;
    }

    [UsedImplicitly]
    public void DropRandom() {
        foreach (var tuple in lootTable.loot) {
            if (!(Random.value < tuple.dropchance)) continue;
            var newLoot = Instantiate(tuple.prefab, transform.position, transform.rotation);
            EnqueueDeliverTask(newLoot);
        }
    }

    private void EnqueueDeliverTask(GameObject newLoot) {
        var storage = _resourceManager.GetBestStorage(newLoot.GetComponent<Storable>());
        var deliveryTask = new DeliveryTask(newLoot, Optional<GameObject>.NoValue(), storage.AsOptional());
        _taskManager.Enqueue(deliveryTask);
    }
}
}