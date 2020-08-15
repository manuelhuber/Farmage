using System;
using Grimity.Singleton;
using UnityEngine;

namespace Features.Time {
public class GameTime : GrimitySingleton<GameTime> {
    [SerializeField] private float _speed = 1;
    public float Time { get; private set; } = 0;

    public float Speed {
        get => _speed;
        set => _speed = value;
    }

    public float DeltaTime => UnityEngine.Time.deltaTime * Speed;

    public float FixedDeltaTime => UnityEngine.Time.fixedDeltaTime * Speed;

    private void FixedUpdate() {
        Time += Speed * UnityEngine.Time.deltaTime;
    }
}
}