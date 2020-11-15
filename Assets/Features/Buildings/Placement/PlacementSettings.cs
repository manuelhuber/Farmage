using UnityEngine;

namespace Features.Buildings.Placement {
[CreateAssetMenu(menuName = "buildings/placement settings")]
public class PlacementSettings : ScriptableObject {
    public Material placementBad;
    public Material placementOk;
}
}