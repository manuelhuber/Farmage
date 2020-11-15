using System;
using System.Collections.Generic;
using Features.Resources;
using UnityEngine;

namespace Features.Buildings.Structures.MerchantDock {
[CreateAssetMenu(menuName = "PurchaseCosts")]
public class MerchantInventory : ScriptableObject {
    public List<PurchaseCost> costs;
}

[Serializable]
public class PurchaseCost {
    public ScriptableObject item;
    public Cost cost;

    /// <summary>
    /// The relative likelihood of the item
    /// </summary>
    public int rollWeight;
}
}