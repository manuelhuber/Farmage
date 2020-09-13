using System.Collections.Generic;
using Features.Save;
using Grimity.Singleton;
using UnityEngine;

namespace Features.Time {
public class GameTime : GrimitySingleton<GameTime>, ISavableComponent<float> {
    [SerializeField] private float _speed = 1;
    public float Time { get; private set; }

    public float Speed {
        get => _speed;
        set => _speed = value;
    }

    public float DeltaTime => UnityEngine.Time.deltaTime * Speed;

    public float FixedDeltaTime => UnityEngine.Time.fixedDeltaTime * Speed;

    private void FixedUpdate() {
        Time += Speed * UnityEngine.Time.fixedDeltaTime;
    }

    public float getTime() {
        return Time;
    }

    #region Save

    public string SaveKey => "GameTime";

    public float Save() {
        return Time;
    }

    public void Load(float data, IReadOnlyDictionary<string, GameObject> objects) {
        Time = data;
    }

    #endregion
}
}