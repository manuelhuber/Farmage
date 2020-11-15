using UnityEngine;
using Vendor.Werewolf.StatusIndicators.Scripts.Base;

namespace Vendor.Werewolf.StatusIndicators.Demos.Scripts {
public class DemoProgressLoop : MonoBehaviour {
    private Splat splat;

    private void Start() {
        splat = GetComponent<Splat>();
    }

    private void Update() {
        splat.Progress = Mathf.PingPong(Time.time * 0.5f, 1);
    }
}
}