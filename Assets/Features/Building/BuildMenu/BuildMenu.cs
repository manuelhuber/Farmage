using System.Collections.Generic;
using UnityEngine;

namespace Features.Building.BuildMenu {
[CreateAssetMenu(menuName = "buildings/building menu")]
public class BuildMenu : ScriptableObject {
    public List<BuildingMenuEntry> entries;
}
}