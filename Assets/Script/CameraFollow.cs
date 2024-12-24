using UnityEngine;
using Unity.Netcode;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // Player that the camera will follow
    public float smoothSpeed = 0.125f; // Smoothness of camera movement
    public Vector3 offset;  // Offset between camera and player

    private float staticYPosition;  // Store the static Y position for the camera

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player not assigned to CameraFollow script.");
        }

        // Store the initial Y position of the camera
        staticYPosition = transform.position.y;
    }

    void LateUpdate()
    {
        if (player != null)
        {
            // Calculate the desired position, keeping Y static
            Vector3 desiredPosition = player.position + offset;
            desiredPosition.y = staticYPosition;  // Set the Y position to remain static

            // Smoothly move the camera
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Set the camera position, keeping Y fixed and updating X and Z
            transform.position = new Vector3(smoothedPosition.x, staticYPosition, smoothedPosition.z);
        }
    }

    // Method to set the player for the camera to follow (called from PlayerMovementTest or elsewhere)
    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }
}