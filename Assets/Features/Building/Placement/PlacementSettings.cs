using UnityEngine;

namespace Features.Building.Placement {
[CreateAssetMenu(menuName = "buildings/placement settings")]
public class PlacementSettings : ScriptableObject {
    public Material placementBad;
    public Material placementOk;
}
}