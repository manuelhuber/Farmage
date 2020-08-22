using System;
using System.Collections.Generic;
using Features.Queue;
using Features.Resources;
using Features.Save;
using Features.Time;
using Ludiq.PeekCore.TinyJson;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.Building.Structures.WheatField {
public class WheatField : MonoBehaviour, ISavableComponent {
    public Cost harvestValue;
    private float _progress;
    [Required] [SerializeField] private JobMultiQueue queue;
    private bool _waitingForHarvest;
    public int growthDurationS;
    private GameTime _time;

    private void Update() {
        if (_waitingForHarvest) return;
        if (_progress >= growthDurationS)
            FinishGrowth();
        else
            UpdateProgress();
    }

    private void Awake() {
        _time = GameTime.Instance;
    }

    private void UpdateProgress() {
        _progress += _time.DeltaTime;
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

    public string SaveKey => "WheatField";

    public string Save() {
        return new WheatFieldData {progress = _progress, waitingForHarvest = _waitingForHarvest}.ToJson();
    }

    public void Load(string rawData, IReadOnlyDictionary<string, GameObject> objects) {
        var data = rawData.FromJson<WheatFieldData>();
        _progress = data.progress;
        _waitingForHarvest = data.waitingForHarvest;
    }
}

[Serializable]
internal struct WheatFieldData {
    public float progress;
    public bool waitingForHarvest;
}
}