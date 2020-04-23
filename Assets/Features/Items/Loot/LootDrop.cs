using Features.Queue;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace Features.Units.common {
public class LootDrop : MonoBehaviour {
    public LootTable lootTable;
    [CanBeNull] public JobMultiQueue lootQueue;

    public void DropRandom() {
        foreach (var tuple in lootTable.loot) {
            if (!(Random.value < tuple.dropchance)) continue;
            var newLoot = Instantiate(tuple.prefab, transform.position, transform.rotation);
            if (lootQueue != null) {
                lootQueue.Enqueue(new Task {type = TaskType.Loot, payload = newLoot});
            }
        }
    }
}
}