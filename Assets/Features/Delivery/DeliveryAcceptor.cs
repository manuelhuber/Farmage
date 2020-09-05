using UnityEngine;

namespace Features.Delivery {
public interface IDeliveryAcceptor {
    bool AcceptDelivery(GameObject goods);
}
}