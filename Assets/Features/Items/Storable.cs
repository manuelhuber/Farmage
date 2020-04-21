using UnityEngine;

namespace Features.Items {
public enum ItemType {
    Fertiliser
}

public class Storable : MonoBehaviour {
    [SerializeField] public ItemType type;

    public bool isType(ItemType test) {
        return type == test;
    }
}
}