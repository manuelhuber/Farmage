using Features.Delivery;
using Features.Queue;
using Features.Resources;
using JetBrains.Annotations;
using UnityEngine;

namespace Features.Items.Loot {
public class LootDrop : MonoBehaviour {
    [CanBeNull] public JobMultiQueue lootQueue;
    public LootTable lootTable;
    private ResourceManager resourceManager;

    private void Awake() {
        resourceManager = ResourceManager.Instance;
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
        var storage = resourceManager.GetBestStorage(newLoot.GetComponent<Storable>());
        if (lootQueue != null) {
            lootQueue.Enqueue(new DeliveryTask {type = TaskType.Loot, Goods = newLoot, Target = storage});
        }
    }
}
}