using System.Collections.Generic;
using System.Linq;
using Features.Health;
using Features.Ui.UserInput;
using Grimity.Math;
using UnityEngine;
using Werewolf.StatusIndicators.Components;

namespace Features.Abilities.AreaDamage {
public class AreaDamageExecutor : AbilityExecutor<AreaDamageAbility>, IOnKeyUp {
    private string SplatName => $"{ability.name} - area damage splat";
    private readonly KeyCode[] _cancelKeys = {KeyCode.Escape, KeyCode.Mouse1};
    private Cone _coneSplat;
    private SplatManager _splatManager;

    private void Start() {
        _splatManager = GetComponentInChildren<SplatManager>();
        _coneSplat = Instantiate(ability.splat, _splatManager.transform);
        _coneSplat.gameObject.name = SplatName;
        _coneSplat.Angle = ability.arc;
        _coneSplat.Scale = 2 * ability.radius;
        _splatManager.Initialize();
    }

    #region InputReceiver

    public event YieldControlHandler YieldControl;

    public void OnKeyUp(HashSet<KeyCode> keys, HashSet<KeyCode> pressedKeys, MouseLocation mouseLocation) {
        if (keys.Contains(KeyCode.Mouse0)) Execute();
        if (_cancelKeys.Any(keys.Contains)) Deactivate();
    }

    #endregion

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
            mortal.TakeDamage(new Damage {Source = gameObject, Amount = ability.damage});
        }

        Deactivate();
    }

    private IEnumerable<Mortal> GetEnemiesInRange() {
        var position = transform.position;
        var casterToSpellIndicator = _coneSplat.Manager.transform.forward;

        bool IsInArc(Collider target) {
            return GeometryUtil.IsInView(position,
                casterToSpellIndicator,
                target.transform.position,
                ability.arc);
        }


        var hits = Physics.OverlapSphere(position, ability.radius);
        return hits
            .Where(IsInArc)
            .Select(hit => hit.transform.gameObject.GetComponent<Mortal>())
            .Where(o => o != null && o.Team != Team.Farmers);
    }
}
}