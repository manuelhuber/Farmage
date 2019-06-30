using System;
using UnityEngine;

namespace Features.Building {
[Serializable]
[CreateAssetMenu(menuName = "buildings/building menu entry")]
public class BuildingMenuEntry : ScriptableObject {
    public GameObject uiPrefab;
    public GameObject buildingPrefab;
    public GameObject previewPrefab;
}
}