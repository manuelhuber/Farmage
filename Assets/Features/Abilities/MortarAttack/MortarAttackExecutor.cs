using System.Collections.Generic;
using System.Linq;
using Features.Health;
using Features.Ui.UserInput;
using UnityEngine;
using Vendor.Werewolf.StatusIndicators.Scripts.Components;

namespace Features.Abilities.MortarAttack {
[RequireComponent(typeof(ITeam))]
public class
    MortarAttackExecutor : AbilityExecutor<MortarAttackAbility>, IKeyDownReceiver, IKeyUpReceiver,
        IInputYielder {
    private string SplatName => ability.name + " Mortar Attack Splat";
    private readonly KeyCode[] _cancelKeys = {KeyCode.Escape, KeyCode.Mouse1};
    private int _alreadyFired;
    private Point _splat;
    private SplatManager _splatManager;
    private Team _team;

    private void Start() {
        _team = GetComponent<ITeam>().Team;
        _splatManager = GetComponentInChildren<SplatManager>();
        _splat = Instantiate(ability.splat, _splatManager.transform);
        _splat.gameObject.name = SplatName;
        _splat.Scale = 2 * ability.radius;
        _splat.Range = ability.range;
        _splat.Progress = 1;
        _splatManager.Initialize();
    }

    #region InputReceiver

    public event YieldControlHandler YieldControl;

    public void OnKeyDown(HashSet<KeyCode> keys, HashSet<KeyCode> pressedKeys, MouseLocation mouseLocation) {
        if (_cancelKeys.Any(keys.Contains)) Deactivate();
    }

    public void OnKeyUp(HashSet<KeyCode> keys, HashSet<KeyCode> pressedKeys, MouseLocation mouseLocation) {
        if (keys.Contains(KeyCode.Mouse0)) FireProjectile();
    }

    #endregion

    public override void Activate() {
        _alreadyFired = 0;
        _splatManager.SelectSpellIndicator(SplatName);
        InputManager.Instance.RequestControlWithMemory(this);
    }

    private void Deactivate() {
        if (_alreadyFired > 0) {
            CalculateNextCooldown();
        }

        _splatManager.CancelSpellIndicator();
        YieldControl?.Invoke(this, new YieldControlEventArgs());
    }

    private void FireProjectile() {
        var target = _splatManager.GetSpellCursorPosition();
        var hits = Physics.OverlapSphere(target, ability.radius);
        var enemies = hits
            .Select(hit => hit.transform.gameObject.GetComponent<Mortal>())
            .Where(enemy => enemy != null && enemy.Team != _team);
        foreach (var enemy in enemies) {
            enemy.TakeDamage(new Damage {Source = gameObject, Amount = ability.damage});
        }

        _alreadyFired++;
        if (_alreadyFired == ability.projectileCount) {
            Deactivate();
        }
    }
}
}