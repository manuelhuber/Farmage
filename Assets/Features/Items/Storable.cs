using UnityEngine;

namespace Features.Items {
public enum ItemType {
    Fertiliser,
    Wheat
}

public class Storable : MonoBehaviour {
    [SerializeField] public ItemType type;

    public bool IsType(ItemType test) {
        return type == test;
    }
}
}