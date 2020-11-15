using System.Collections.Generic;
using UnityEngine;

namespace Features.Buildings.BuildMenu {
[CreateAssetMenu(menuName = "buildings/building menu")]
public class BuildMenu : ScriptableObject {
    public List<BuildingMenuEntry> entries;
}
}