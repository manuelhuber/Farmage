using System;
using Features.Buildings.UI;
using Features.Enemies;
using Features.Pathfinding;
using Features.Resources;
using Features.Save;
using Features.Time;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Features.Mission {
public class MissionManager : MonoBehaviour {
    public GameObject workerPrefab;
    private string _startTime;

    private void Awake() {
        _startTime = DateTime.Now.ToString("yyyyMMddHHmmss");
        var settings = UnityEngine.Resources.Load<MissionSettings>("NormalMission");
        if (settings != null) {
            InitialiseMission(settings);
        }
    }

    public void EndMission() {
        var summary = new MissionSummary
            {time = GameTime.Instance.Time, enemiesKilled = EnemyManager.Instance.DeathCount};
        SaveFile.Write(summary, $"/history/{_startTime}.json");
        SceneManager.LoadScene("MainMenu");
    }

    private void InitialiseMission(MissionSettings settings) {
        var buildingManager = BuildingManager.Instance;
        foreach (var building in settings.startingBuildings) {
            buildingManager.AddBuildingOption(building);
        }

        ResourceManager.Instance.startingCash = settings.startingMoney;
        var mapManager = MapManager.Instance;
        mapManager.sizeX = settings.mapSize.x;
        mapManager.sizeZ = settings.mapSize.y;
        Instantiate(settings.walkerPrefab, Vector3.zero, Quaternion.identity);

        for (var i = 0; i < settings.workerCount; i++) {
            Instantiate(workerPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}
}