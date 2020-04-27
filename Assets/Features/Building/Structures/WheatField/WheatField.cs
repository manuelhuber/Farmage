using Features.Queue;
using Features.Resources;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.Building.Structures.WheatField {
public class WheatField : MonoBehaviour {
    public Cost harvestValue;
    private float _progress;
    [Required] [SerializeField] private JobMultiQueue _queue;
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
        _queue.Enqueue(new Task {payload = gameObject, type = TaskType.Harvest});
    }

    public Cost harvest() {
        if (!_waitingForHarvest) return new Cost();
        _waitingForHarvest = false;
        _progress = 0;
        return harvestValue;
    }
}
}