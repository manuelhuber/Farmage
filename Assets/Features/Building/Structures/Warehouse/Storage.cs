using System;
using System.Collections.Generic;
using System.Linq;
using Features.Delivery;
using Features.Items;
using Features.Save;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;

namespace Features.Building.Structures.Warehouse {
public class Storage : DeliveryAcceptor, ISavableComponent {
    public ItemType type;
    public int capacity;
    public List<Storable> items = new List<Storable>();
    public int size = 10;

    public bool IsFull => items.Count == capacity;

    public string SaveKey => "Storage";

    public string Save() {
        return new StorageData {items = items.Select(storable => storable.getSaveID()).ToArray()}.ToJson();
    }

    public void Load(string rawData, IReadOnlyDictionary<string, GameObject> objects) {
        foreach (var itemID in rawData.FromJson<StorageData>().items) {
            var item = objects[itemID];
            AcceptDelivery(item);
        }
    }


    public override bool AcceptDelivery(GameObject goods) {
        var item = goods.GetComponent<Storable>();
        if (!CanAccept(item)) return false;
        items.Add(item);
        PlaceItemInStorage(item);
        return true;
    }

    private void PlaceItemInStorage(Component item) {
        var count = items.Count;
        var itemObject = item.gameObject;
        itemObject.transform.parent = transform;
        var planeCount = size * size * 1f;
        var offset = size / 2f - 0.5f;
        var y = count / planeCount + 0.5f;
        var x = count % planeCount / size - offset;
        var z = count % size - offset;
        itemObject.transform.localPosition = new Vector3(x, y, z);
    }

    public bool CanAccept(Storable item) {
        return !IsFull && item.IsType(type);
    }
}

[Serializable]
internal struct StorageData {
    public string[] items;
}
}