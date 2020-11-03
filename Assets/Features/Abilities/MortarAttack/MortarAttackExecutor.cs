using System;
using System.Collections.Generic;
using System.Linq;
using Features.Health;
using Features.Ui.UserInput;
using UnityEngine;
using Werewolf.StatusIndicators.Components;

namespace Features.Abilities.MortarAttack {
public class MortarAttackExecutor : AbilityExecutor, IInputReceiver {
    private string SplatName => _ability.name + " Mortar Attack Splat";

    public override bool CanActivate => true;
    private readonly KeyCode[] _cancelKeys = {KeyCode.Escape, KeyCode.Mouse1};
    private MortarAttackAbility _ability;
    private int _alreadyFired;
    private Point _splat;
    private SplatManager _splatManager;

    #region InputReceiver

    public event EventHandler YieldControl;

    public void OnKeyDown(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
        if (_cancelKeys.Any(keys.Contains)) Deactivate();
    }

    public void OnKeyUp(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
        if (keys.Contains(KeyCode.Mouse0)) FireProjectile();
    }

    public void OnKeyPressed(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
    }

    #endregion

    public override void Init(Ability ability) {
        _splatManager = GetComponentInChildren<SplatManager>();
        _ability = ability as MortarAttackAbility;
        _splat = Instantiate(_ability.splat, _splatManager.transform);
        _splat.gameObject.name = SplatName;
        _splat.Scale = 2 * _ability.radius;
        _splat.Progress = 1;
        _splatManager.Initialize();
    }

    public override void Activate() {
        _alreadyFired = 0;
        _splatManager.SelectSpellIndicator(SplatName);
        InputManager.Instance.RequestControlWithMemory(this);
    }

    private void Deactivate() {
        _splatManager.CancelSpellIndicator();
        YieldControl?.Invoke(this, EventArgs.Empty);
    }

    private void FireProjectile() {
        var target = _splatManager.GetSpellCursorPosition();
        var hits = Physics.OverlapSphere(target, _ability.radius);
        var enemies = hits
            .Select(hit => hit.transform.gameObject.GetComponent<Mortal>())
            .Where(o => o != null && o.team != Team.Farmers);
        foreach (var enemy in enemies) {
            enemy.TakeDamage(_ability.damage);
        }

        _alreadyFired++;
        if (_alreadyFired == _ability.projectileCount) {
            Deactivate();
        }
    }
}
}