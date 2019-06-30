using System.Collections.Generic;
using UnityEngine;

namespace Features.Building.Structures {
[CreateAssetMenu(menuName = "buildings/building menu")]
public class BuildMenu : ScriptableObject {
    public List<BuildingMenuEntry> entries;
    public Material placementOk;
    public Material placementBad;
}
}