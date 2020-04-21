using System.Collections.Generic;
using Features.Items;
using UnityEngine;

namespace Features.Building.Structures.Warehouse {
public class Storage : MonoBehaviour {
    public ItemType _type;
    public int capacity;
    public int size = 10;
    public List<Storable> items = new List<Storable>();

    public void Deliver(Storable item) {
        if (!item.isType(_type)) return;
        var count = items.Count;
        items.Add(item);
        item.gameObject.transform.parent = transform;
        var planeCount = size * size;
        var offset = size / 2 - 0.5f;
        var y = (count / planeCount) + 0.5f;
        var x = (count % planeCount / size) - offset;
        var z = (count % size) - offset;
        item.gameObject.transform.localPosition = new Vector3(x, y, z);
    }
}
}