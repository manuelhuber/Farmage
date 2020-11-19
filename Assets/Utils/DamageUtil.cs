using System.Collections.Generic;
using System.Linq;
using Features.Attacks.Damage;
using Features.Health;
using UnityEngine;

namespace Features.Abilities.MortarAttack {
public class DamageUtil {
    public static IEnumerable<Mortal> DamageEnemies(Vector3 target,
                                                    float radius,
                                                    Damage damage,
                                                    Team attacker) {
        var hits = Physics.OverlapSphere(target, radius);
        var enemies = hits
            .Select(hit => hit.transform.gameObject.GetComponent<Mortal>())
            .Where(enemy => enemy != null && enemy.Team != attacker)
            .ToList();
        foreach (var enemy in enemies) {
            enemy.TakeDamage(damage);
        }

        return enemies;
    }
}
}