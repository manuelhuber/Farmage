using System.Linq;
using Features.Pathfinding;
using Features.Resources;
using UnityEngine;

namespace Features.Mission {
public class MissionInitializer : MonoBehaviour {
    public GameObject workerPrefab;
    private MissionSettings _settings;

    private void Awake() {
        _settings = UnityEngine.Resources.FindObjectsOfTypeAll<MissionSettings>().First();
        ResourceManager.Instance.startingCash = _settings.startingMoney;
        MapManager.Instance.sizeX = _settings.mapSize.x;
        MapManager.Instance.sizeZ = _settings.mapSize.y;
        Instantiate(_settings.walkerPrefab, Vector3.zero, Quaternion.identity);

        for (var i = 0; i < _settings.workerCount; i++) {
            Instantiate(workerPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}
}