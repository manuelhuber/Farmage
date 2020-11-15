using UnityEngine;

namespace Vendor.Werewolf.StatusIndicators.Demos.Scripts {
public class CameraMovement : MonoBehaviour {
    public GameObject player;
    public float offsetX = -5;
    public float offsetZ;
    public float maximumDistance = 2;
    public float playerVelocity = 10;

    private float movementX;
    private float movementZ;

    private void Update() {
        movementX = (player.transform.position.x + offsetX - transform.position.x) / maximumDistance;
        movementZ = (player.transform.position.z + offsetZ - transform.position.z) / maximumDistance;
        transform.position += new Vector3(movementX * playerVelocity * Time.deltaTime,
            0,
            movementZ * playerVelocity * Time.deltaTime);
    }
}
}