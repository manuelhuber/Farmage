using System;
using Features.Resources;
using Unity.Mathematics;
using UnityEngine;

namespace Features.Building.BuildMenu {
[Serializable]
[CreateAssetMenu(menuName = "buildings/building menu entry")]
public class BuildingMenuEntry : ScriptableObject {
    public GameObject buildingPrefab;
    public GameObject modelPrefab;
    public int2 size;
    public Cost cost;
    public Sprite image;
}
}