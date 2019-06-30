using UnityEngine;

namespace Features.Building.Placement {
[CreateAssetMenu(menuName = "buildings/placement settings")]
public class PlacementSettings : ScriptableObject {
    public Material placementOk;
    public Material placementBad;
    public float placementThreshold;
}
}