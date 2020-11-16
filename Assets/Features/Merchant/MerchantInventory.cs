using System.Collections.Generic;
using UnityEngine;

namespace Features.Merchant {
[CreateAssetMenu(menuName = "Merchant/Inventory")]
public class MerchantInventory : ScriptableObject {
    public List<MerchantEntry> costs;
}
}