using System;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Time {
public class GameTimeUi : MonoBehaviour {
    public Text gameTime;

    private GameTime _time;

    private void Awake() {
        _time = GameTime.Instance;
    }

    private void Update() {
        gameTime.text = GetFormattedGameTime();
    }

    private void SetSpeed(float speed) {
        _time.Speed = speed;
    }

    public void Pause() {
        SetSpeed(0);
    }

    public void Normal() {
        SetSpeed(1);
    }

    public void Double() {
        SetSpeed(2);
    }

    private string GetFormattedGameTime() {
        var time = Math.Round(_time.Time);
        var minutes = Math.Floor(time / 60);
        var seconds = time % 60;
        string Pad(double num) => num < 10 ? $"0{num}" : num.ToString();
        return $"{Pad(minutes)}:{Pad(seconds)}";
    }
}
}