using UnityEngine;

namespace Vendor.Werewolf.StatusIndicators.Scripts.Effects {
public class RadialDistort : MonoBehaviour {
    public float Speed;
    private Material Material;

    private void Start() {
        Material = GetComponent<Projector>().material;
    }

    private void Update() {
        Material.SetFloat("_Offset", Mathf.Repeat(Time.time * Speed, 1));
    }
}
}