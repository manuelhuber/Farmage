using System;
using Features.Resources;
using UnityEngine;

namespace Features.Building.BuildMenu {
[Serializable]
[CreateAssetMenu(menuName = "buildings/building menu entry")]
public class BuildingMenuEntry : ScriptableObject {
    public GameObject buildingPrefab;
    public Cost cost;
    public Sprite image;
    public GameObject previewPrefab;
}
}