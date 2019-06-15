using System;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace UnityEngine {
public class BuildingInput : MonoBehaviour {
    public GameObject BuildingUIPrefab;
    private Canvas _canvas;
    private GameObject _UiCore;

    private void OnEnable() {
        _canvas = FindObjectOfType<Canvas>();
        _UiCore = Instantiate(new GameObject(), _canvas.transform, false);
        _UiCore.name = "Mouse Centric UI";
        var button = Instantiate(BuildingUIPrefab, _UiCore.transform, false);
        button.transform.localPosition = new Vector3(50, 0, 0);
        var buttonClickedEvent = new Button.ButtonClickedEvent();
        buttonClickedEvent.AddListener(() => { Debug.Log("Foo"); });
        button.GetComponent<UI.Button>().onClick = buttonClickedEvent;


        _UiCore.SetActive(false);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            _UiCore.SetActive(true);
            _UiCore.transform.position = Input.mousePosition;
        } else if (Input.GetKeyDown(KeyCode.Escape)) {
            _UiCore.SetActive(false);
        }
    }
}
}