using UnityEngine;
using UnityEngine.UI;

namespace Vendor.Werewolf.StatusIndicators.Demos.SpellIndicators.Scripts {
public class SplatName : MonoBehaviour {
    private void OnEnable() {
        GetComponent<Text>().text = "";
    }
}
}