using Features.Resources;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.Buildings.Production {
[CreateAssetMenu(menuName = "production/Unit production")]
public class UnitProductionEntry : ScriptableObject {
    [Required] public GameObject prefab;
    public Sprite icon;
    [Required] public float productionTimeInSeconds;
    [Required] public Cost cost;
}
}