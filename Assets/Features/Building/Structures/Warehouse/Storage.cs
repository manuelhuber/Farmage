using System.Collections.Generic;
using Features.Items;
using UnityEngine;

namespace Features.Building.Structures.Warehouse {
public class Storage : MonoBehaviour {
    public ItemType type;
    public int capacity;
    public List<Storable> items = new List<Storable>();
    public int size = 10;

    public void Deliver(Storable item) {
        if (!item.IsType(type)) return;
        var count = items.Count;
        items.Add(item);
        item.gameObject.transform.parent = transform;
        var planeCount = size * size * 1f;
        var offset = size / 2f - 0.5f;
        var y = count / planeCount + 0.5f;
        var x = count % planeCount / size - offset;
        var z = count % size - offset;
        item.gameObject.transform.localPosition = new Vector3(x, y, z);
    }
}
}