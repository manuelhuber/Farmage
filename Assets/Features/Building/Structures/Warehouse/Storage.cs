using System;
using System.Collections.Generic;
using System.Linq;
using Features.Delivery;
using Features.Items;
using Features.Save;
using Grimity.Data;
using Ludiq.PeekCore.TinyJson;
using UnityEngine;

namespace Features.Building.Structures.Warehouse {
public class Storage : MonoBehaviour, IDeliveryAcceptor, IDeliveryDispenser, ISavableComponent {
    public ItemType type;
    public int capacity;
    public int size = 10;

    public bool IsFull => items.Count == capacity;

    private readonly List<StoredItem> items = new List<StoredItem>();

    public bool AcceptDelivery(GameObject goods) {
        var item = goods.GetComponent<Storable>();
        if (!CanAccept(item)) return false;
        items.Add(new StoredItem {Storable = item});
        PlaceItemInStorage(item);
        return true;
    }

    public bool DispenseDelivery(GameObject goods) {
        var removedItems = items.RemoveAll(item => item.Storable.gameObject == goods);
        return removedItems > 0;
    }

    public Optional<Storable> ReserveItem(Predicate<Storable> predicate) {
        var storedItem = items.Where(item => !item.Reserved).FirstOrDefault(item => predicate(item.Storable));
        if (storedItem.Storable == null) return Optional<Storable>.NoValue();
        var index = items.IndexOf(storedItem);
        storedItem.Reserved = true;
        items[index] = storedItem;
        return Optional<Storable>.Of(storedItem.Storable);
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


    private struct StoredItem {
        public Storable Storable;
        public bool Reserved;
    }

    #region Save

    public string SaveKey => "Storage";

    public string Save() {
        var serializedItems = items.Select(item => new StoredItemData {
                storable = item.Storable.getSaveID(),
                reserved = item.Reserved
            })
            .ToArray();
        return new StorageData {items = serializedItems}.ToJson();
    }

    public void Load(string rawData, IReadOnlyDictionary<string, GameObject> objects) {
        foreach (var item in rawData.FromJson<StorageData>().items) {
            var storable = objects[item.storable].GetComponent<Storable>();
            items.Add(new StoredItem {Reserved = item.reserved, Storable = storable});
        }
    }

    [Serializable]
    private struct StorageData {
        public StoredItemData[] items;
    }

    [Serializable]
    private struct StoredItemData {
        public string storable;
        public bool reserved;
    }

    #endregion
}
}