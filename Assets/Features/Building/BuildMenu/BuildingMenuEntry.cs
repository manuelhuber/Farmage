using System;
using Features.Resources;
using UnityEngine;

namespace Features.Building.BuildMenu {
[Serializable]
[CreateAssetMenu(menuName = "buildings/building menu entry")]
public class BuildingMenuEntry : ScriptableObject {
    public Sprite image;
    public GameObject buildingPrefab;
    public GameObject previewPrefab;
    public Cost cost;
}
}