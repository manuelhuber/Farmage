using UnityEngine;
using UnityEngine.SceneManagement;

namespace Features.Menu {
public class MainMenu : MonoBehaviour {
    public void StartMission() {
        SceneManager.LoadScene("Tutorial");
    }
}
}