using Constants;
using Features.Health;
using Features.Time;
using Grimity.Actions;
using UnityEngine;

namespace Features.Building.Structures.FieldTower {
public class FieldTower : MonoBehaviour {
    public GameObject spherePrefab;
    public int regenPerSecond;
    public float sphereRebuildDelay = 1f;
    public int sphereHp;
    private DelayedAction _rebuildAction;

    private PeriodicalAction _shieldRegeneration;
    private GameTime _time;

    private void Awake() {
        _time = GameTime.Instance;
        _shieldRegeneration = gameObject.AddComponent<PeriodicalAction>();
        _shieldRegeneration.getTime = () => _time.Time;
        _shieldRegeneration.interval = 1;
    }

    private void Start() {
        RebuildSphere();
    }

    private void RebuildSphere() {
        _rebuildAction = this.Do(() => BuildSphere(sphereHp)).withTime(_time.getTime);
        _rebuildAction.After(sphereRebuildDelay);
    }

    private void BuildSphere(int initialHp) {
        _rebuildAction = null;
        var sphere = Instantiate(spherePrefab, transform).AddComponent<Mortal>();
        sphere.tag = Tags.SphereTag;
        sphere.MaxHitpoints = sphereHp;
        sphere.TakeDamage(new Damage {Source = gameObject, Amount = -initialHp});
        sphere.onDeath.AddListener(() => {
            _shieldRegeneration.action = null;
            RebuildSphere();
        });
        _shieldRegeneration.action = () => {
            sphere.TakeDamage(new Damage {Source = gameObject, Amount = -regenPerSecond});
            return true;
        };
    }
}
}