using Features.Queue;
using JetBrains.Annotations;
using UnityEngine;

namespace Features.Units.common {
public class LootDrop : MonoBehaviour {
    [CanBeNull] public JobMultiQueue lootQueue;
    public LootTable lootTable;

    public void DropRandom() {
        foreach (var tuple in lootTable.loot) {
            if (!(Random.value < tuple.dropchance)) continue;
            var newLoot = Instantiate(tuple.prefab, transform.position, transform.rotation);
            if (lootQueue != null) lootQueue.Enqueue(new Task {type = TaskType.Loot, payload = newLoot});
        }
    }
}
}