using System.Collections.Generic;
using Features.Items;
using Features.Resources;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.Building.Structures.MerchantDock {
[CreateAssetMenu(fileName = "Sales Table", menuName = "sales", order = 0)]
public class SalesPrices : SerializedScriptableObject {
    public Dictionary<ItemType, Cost> Prices;
}
}