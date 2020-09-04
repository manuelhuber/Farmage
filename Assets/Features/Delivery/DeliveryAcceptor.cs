using UnityEngine;

namespace Features.Delivery {
public abstract class DeliveryAcceptor : MonoBehaviour {
    public abstract bool AcceptDelivery(GameObject goods);
}
}