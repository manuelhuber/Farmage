using System.Collections.Generic;
using System.Linq;
using Features.Health;
using Features.Ui.UserInput;
using UnityEngine;
using Werewolf.StatusIndicators.Components;

namespace Features.Abilities.AreaDamage {
public class AreaDamageExecutor : AbilityExecutor<AreaDamageAbility>, IInputReceiver {
    private string SplatName => $"{_ability.name} - area damage splat";

    private readonly KeyCode[] _cancelKeys = {KeyCode.Escape, KeyCode.Mouse1};

    private AreaDamageAbility _ability;
    private Cone _coneSplat;
    private SplatManager _splatManager;

    #region InputReceiver

    public event YieldControlHandler YieldControl;

    public void OnKeyDown(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
    }

    public void OnKeyUp(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
        if (keys.Contains(KeyCode.Mouse0)) Execute();
        if (_cancelKeys.Any(keys.Contains)) Deactivate();
    }

    public void OnKeyPressed(HashSet<KeyCode> keys, MouseLocation mouseLocation) {
    }

    #endregion

    protected override void InitImpl(AreaDamageAbility ability) {
        _splatManager = GetComponentInChildren<SplatManager>();
        _ability = ability;
        _coneSplat = Instantiate(_ability.splat, _splatManager.transform);
        _coneSplat.gameObject.name = SplatName;
        _coneSplat.Angle = _ability.arc;
        _coneSplat.Scale = 2 * _ability.radius;
        _splatManager.Initialize();
    }

    public override void Activate() {
        InputManager.Instance.RequestControlWithMemory(this);
        _splatManager.SelectSpellIndicator(SplatName);
    }

    private void Deactivate() {
        _splatManager.CancelSpellIndicator();
        YieldControl?.Invoke(this, new YieldControlEventArgs());
    }

    private void Execute() {
        DealDamage();
        CalculateNextCooldown();
    }

    private void DealDamage() {
        var enemies = GetEnemiesInRange();
        foreach (var mortal in enemies) {
            mortal.TakeDamage(_ability.damage);
        }

        Deactivate();
    }

    private IEnumerable<Mortal> GetEnemiesInRange() {
        var position = transform.position;
        var casterToSpellIndicator = _coneSplat.Manager.transform.forward;

        bool IsInArc(Collider target) {
            var casterToTarget = (target.transform.position - position).normalized;
            var rightAngle = _ability.arc / 2;
            var leftAngle = 360 - rightAngle;
            var angle = Vector3.Angle(casterToSpellIndicator, casterToTarget);
            var isInArc = angle <= rightAngle || angle >= leftAngle;
            return isInArc;
        }


        var hits = Physics.OverlapSphere(position, _ability.radius);
        return hits
            .Where(IsInArc)
            .Select(hit => hit.transform.gameObject.GetComponent<Mortal>())
            .Where(o => o != null && o.team != Team.Farmers);
    }
}
}