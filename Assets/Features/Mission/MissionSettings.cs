using Features.Resources;
using UnityEngine;

namespace Features.Mission {
[CreateAssetMenu(menuName = "mission/settings")]
public class MissionSettings : ScriptableObject {
    public Cost startingMoney;
    public Vector2 mapSize;
    public int workerCount;
    public GameObject walkerPrefab;
}
}