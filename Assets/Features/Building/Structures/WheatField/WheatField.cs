﻿using Features.Queue;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.Building.Structures.WheatField {
public class WheatField : MonoBehaviour {
    public int growthDurationS;
    private float _progress;
    [Required] [SerializeField] private JobMultiQueue _queue;
    private bool _waitingForHarvest = false;
    private int _harvestValue = 100;

    private void Update() {
        if (_waitingForHarvest) return;
        if (_progress >= growthDurationS) {
            FinishGrowth();
        } else {
            UpdateProgress();
        }
    }

    private void UpdateProgress() {
        _progress += Time.deltaTime;
    }

    private void FinishGrowth() {
        Debug.Log("ready for harvest");
        _waitingForHarvest = true;
        _queue.Enqueue(new Task {payload = gameObject, type = TaskType.Harvest});
    }

    public int harvest() {
        _waitingForHarvest = false;
        _progress = 0;
        return _harvestValue;
    }
}
}