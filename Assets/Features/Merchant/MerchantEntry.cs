using Features.Resources;
using UnityEngine;

namespace Features.Merchant {
[CreateAssetMenu(menuName = "Merchant/Item")]
public class MerchantEntry : ScriptableObject {
    public ScriptableObject item;
    public Cost cost;

    /// <summary>
    ///     The relative likelihood of the item
    /// </summary>
    public int rollWeight;
}
}