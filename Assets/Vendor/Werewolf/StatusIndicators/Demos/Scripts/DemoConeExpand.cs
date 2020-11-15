using UnityEngine;
using Vendor.Werewolf.StatusIndicators.Scripts.Components;

namespace Vendor.Werewolf.StatusIndicators.Demos.Scripts {
public class DemoConeExpand : MonoBehaviour {
    private Cone spellIndicator;

    private void Start() {
        spellIndicator = GetComponent<Cone>();
    }

    private void Update() {
        spellIndicator.Angle = Mathf.PingPong(Time.time * 100f, 320f) + 40f;
    }
}
}