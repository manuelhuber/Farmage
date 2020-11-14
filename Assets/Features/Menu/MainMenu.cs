using Features.Mission;
using Features.Resources;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Features.Menu {
public class MainMenu : MonoBehaviour {
    public GameObject walkerPrefab;

    public void StartMission() {
        GenerateMissionSettings();
        SceneManager.LoadScene("Tutorial");
    }

    private void GenerateMissionSettings() {
        var settings = ScriptableObject.CreateInstance<MissionSettings>();
        settings.startingMoney = new Cost {cash = 400};
        settings.walkerPrefab = walkerPrefab;
        settings.mapSize = new Vector2 {x = 500, y = 500};
        settings.workerCount = 1;
    }
}
}