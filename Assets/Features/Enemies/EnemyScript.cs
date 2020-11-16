using Features.Animations;
using Features.Health;
using Features.Units.Common;
using Grimity.Data;
using UnityEngine;

namespace Features.Enemies {
[RequireComponent(typeof(AdvancedMovementController))]
[RequireComponent(typeof(Mortal))]
public class EnemyScript : MonoBehaviour {
    [SerializeField] public Mortal initialTarget;

    public Mortal DefaultTarget {
        set {
            _defaultTarget = value.AsOptional();
            UpdateTarget();
        }
    }

    private Optional<Mortal> _defaultTarget;
    private AdvancedMovementController _movementController;

    private Optional<Mortal> _threateningTarget = Optional<Mortal>.NoValue();


    private void Awake() {
        _movementController = GetComponent<AdvancedMovementController>();

        GetComponent<Mortal>().OnDamageInterceptor.Add(UpdateThreateningTarget);
        if (initialTarget != null) {
            DefaultTarget = initialTarget;
        }
    }

    private void Start() {
        var animator = gameObject.GetComponentInChildren<AnimationHandler>();
        _movementController.IsMoving.OnChange(isMoving => animator.SetBool("isMoving", isMoving));
    }


    private Damage UpdateThreateningTarget(Damage damage) {
        if (_threateningTarget.HasValue || damage.Source == null) return damage;
        var mortal = damage.Source.GetComponent<Mortal>();
        if (mortal == null) return damage;
        SetHighPriorityTarget(mortal);

        return damage;
    }

    public void SetHighPriorityTarget(Mortal mortal) {
        _threateningTarget = mortal.AsOptional();
        mortal.onDeath.AddListener(() => {
            _threateningTarget = Optional<Mortal>.NoValue();
            UpdateTarget();
        });
        UpdateTarget();
    }

    private void UpdateTarget() {
        if (_threateningTarget.HasValue) {
            _movementController.ChaseTarget(_threateningTarget.Value.transform);
        } else if (_defaultTarget.HasValue) {
            _movementController.AttackMoveTo(_defaultTarget.Value.transform.position);
        }
    }
}
}