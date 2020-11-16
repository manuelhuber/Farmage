using Features.Health;
using Features.Time;
using UnityEngine;

namespace Features.Buildings.Structures.Scarecrow {
public class Regeneration : MonoBehaviour {
    public int hitpointAmount;
    public int tickRateInS = 1;
    private GameTime _gameTime;

    private Mortal _mortal;
    private float _nextTick;

    private void Awake() {
        _gameTime = GameTime.Instance;
        _mortal = GetComponent<Mortal>();
    }

    private void Update() {
        if (_gameTime.getTime() > _nextTick) {
            Tick();
        }
    }

    private void Tick() {
        _mortal.TakeDamage(new Damage {Amount = -hitpointAmount});
        _nextTick += tickRateInS;
    }
}
}