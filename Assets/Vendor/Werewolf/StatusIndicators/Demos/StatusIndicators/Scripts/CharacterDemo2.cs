using UnityEngine;
using UnityEngine.UI;
using Vendor.Werewolf.StatusIndicators.Demos.SpellIndicators.Scripts;
using Vendor.Werewolf.StatusIndicators.Scripts.Components;

namespace Vendor.Werewolf.StatusIndicators.Demos.StatusIndicators.Scripts {
public class CharacterDemo2 : MonoBehaviour {
    public SplatManager Splats { get; set; }

    private int index;

    private void Start() {
        Splats = GetComponentInChildren<SplatManager>();
        Splats.SelectStatusIndicator(Splats.StatusIndicators[0].name);
        UpdateSelection();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Splats.CancelStatusIndicator();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            index = (int) Mathf.Repeat(index - 1, Splats.StatusIndicators.Length);
            UpdateSelection();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            index = (int) Mathf.Repeat(index + 1, Splats.StatusIndicators.Length);
            UpdateSelection();
        }
    }

    private void UpdateSelection() {
        Splats.SelectStatusIndicator(Splats.StatusIndicators[index].name);
        FindObjectOfType<SplatName>().GetComponent<Text>().text =
            index + ": " + Splats.CurrentStatusIndicator.name;
    }
}
}