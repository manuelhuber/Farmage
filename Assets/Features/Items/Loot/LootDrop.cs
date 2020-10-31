using Features.Resources;
using Features.Tasks;
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
        // TODO
    }
}
}