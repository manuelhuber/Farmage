using System;
using System.Collections.Generic;
using Features.Health;
using Features.Save;
using Features.Units.Common;
using Grimity.Data;
using UnityEngine;

namespace Features.Units.Enemies {
[RequireComponent(typeof(AdvancedMovementController))]
[RequireComponent(typeof(Mortal))]
public class EnemyScript : MonoBehaviour, ISavableComponent<EnemyData> {
    public Mortal DefaultTarget {
        set {
            _defaultTarget = value.AsOptional();
            UpdateTarget();
        }
    }

    private Optional<Mortal> _threateningTarget = Optional<Mortal>.NoValue();
    private AdvancedMovementController _movementController;
    private Optional<Mortal> _defaultTarget;


    private void Awake() {
        _movementController = GetComponent<AdvancedMovementController>();
        GetComponent<Mortal>().OnDamageInterceptor.Add(OnDamage);
    }


    private Damage OnDamage(Damage damage) {
        if (!_threateningTarget.HasValue && damage.Source != null) {
            var mortal = damage.Source.GetComponent<Mortal>();
            if (mortal != null) {
                _threateningTarget = mortal.AsOptional();
                mortal.onDeath.AddListener(() => {
                    _threateningTarget = Optional<Mortal>.NoValue();
                    UpdateTarget();
                });
                UpdateTarget();
            }
        }

        return damage;
    }

    private void UpdateTarget() {
        if (_threateningTarget.HasValue) {
            _movementController.ChaseTarget(_threateningTarget.Value.transform);
        } else if (_defaultTarget.HasValue) {
            _movementController.AttackMoveTo(_defaultTarget.Value.transform.position);
        }
    }


    #region Save

    public string SaveKey => "EnemyController";

    public EnemyData Save() {
        return new EnemyData();
    }

    public void Load(EnemyData data, IReadOnlyDictionary<string, GameObject> objects) {
    }

    #endregion
}

[Serializable]
public struct EnemyData {
}
}