using UnityEngine;

namespace Vendor.Werewolf.StatusIndicators.Scripts.Effects {
public class LinearDistort : MonoBehaviour {
    public float XSpeed, YSpeed;
    private Material Material;

    private void Start() {
        Material = GetComponent<Projector>().material;
    }

    private void Update() {
        Material.SetFloat("_OffsetX", Mathf.Repeat(Time.time * XSpeed, 1));
        Material.SetFloat("_OffsetY", Mathf.Repeat(Time.time * YSpeed, 1));
    }
}
}