using Features.Resources;
using UnityEngine;

namespace Features.Building.Production {
[CreateAssetMenu(menuName = "production/Unit production")]
public class UnitProductionEntry : ScriptableObject {
    public GameObject prefab;
    public Sprite icon;
    public float productionTimeInSeconds;
    public Cost cost;
}
}