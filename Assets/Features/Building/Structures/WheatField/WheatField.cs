using Features.Queue;
using Features.Resources;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.Building.Structures.WheatField {
public class WheatField : MonoBehaviour {
    public Cost harvestValue;
    private float _progress;
    [Required] [SerializeField] private JobMultiQueue queue;
    private bool _waitingForHarvest;
    public int growthDurationS;

    private void Update() {
        if (_waitingForHarvest) return;
        if (_progress >= growthDurationS)
            FinishGrowth();
        else
            UpdateProgress();
    }

    private void UpdateProgress() {
        _progress += Time.deltaTime;
    }

    private void FinishGrowth() {
        _waitingForHarvest = true;
        queue.Enqueue(new Task {payload = gameObject, type = TaskType.Harvest});
    }

    public Cost Harvest() {
        if (!_waitingForHarvest) return new Cost();
        _waitingForHarvest = false;
        _progress = 0;
        return harvestValue;
    }
}
}