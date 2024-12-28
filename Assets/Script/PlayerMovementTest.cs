using UnityEngine;
using Unity.Netcode;

public class PlayerMovementTest : NetworkBehaviour
{
    public NetworkVariable<Vector2> Position = new NetworkVariable<Vector2>(
        new Vector2(0f, 0f), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public GameObject bulletPrefab; // Bullet prefab
    public Transform firePoint;    // Fire point for shooting
    public float bulletSpeed = 20f; // Speed of the bullet

    public float moveSpeed = 3f;   // Movement speed
    public float jumpForce = 7f; // Jump force
    private Rigidbody2D rb;       // Rigidbody for physics-based movement
    private bool isGrounded = true;
    private bool isGravityInverted = false; // Track gravity state
    private Vector3 originalGravity;  // Original gravity direction

    public Vector3 startPosition;
    private CameraFollow cameraFollow;

    // Animation reference
    private Animation playerAnimation;

    // NetworkVariable to sync animation state across the network
    private NetworkVariable<bool> isWalking = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Start()
    {
        startPosition = transform.position;
        originalGravity = Physics2D.gravity;  // Store the original gravity direction

        // Get the Animation component
        playerAnimation = GetComponent<Animation>();
    }

    public void ReturnToStart()
    {
        transform.position = startPosition;
        Position.Value = startPosition; // Sync position with the network
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            rb = GetComponent<Rigidbody2D>();

            // Find and assign the camera to follow this player
            cameraFollow = Camera.main.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.SetPlayer(transform);  // Set this player's transform for the camera to follow
            }
        }

        Position.OnValueChanged += OnPositionChanged;
        isWalking.OnValueChanged += OnWalkingStateChanged; // Listen for walking state changes
    }

    void Update()
    {
        if (IsOwner)
        {
            Move();  // Handle movement and jumping

            // Handle shooting
            if (Input.GetButtonDown("Fire1")) // Left mouse button
            {
                ShootServerRpc();
            }

            // Toggle gravity on key press
            if (Input.GetKeyDown(KeyCode.G)) // Change this key to whatever you'd like
            {
                ToggleGravity();
            }

            // Sync position if it has changed
            if (Vector2.Distance(Position.Value, rb.position) > 0.1f)
            {
                UpdatePositionServerRpc(rb.position);
            }

            // Check if walking animation needs to be updated
            UpdateWalkingAnimation();
        }
        else
        {
            transform.position = new Vector3(Position.Value.x, Position.Value.y, 0f); // Update position from network
        }
    }

    // Shooting logic
    [ServerRpc]
    private void ShootServerRpc(ServerRpcParams rpcParams = default)
    {
        ShootClientRpc();
    }

    [ClientRpc]
    private void ShootClientRpc()
    {
        if (firePoint != null && bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = firePoint.right * bulletSpeed; // Shoot in the direction of the fire point

            // Destroy the bullet after 2 seconds to avoid clutter
            Destroy(bullet, 2f);
        }
    }

    // Public method to allow other scripts to call Move()
    public void Move()
    {
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = movement;

        // Update walking state based on movement
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            if (!isWalking.Value) // Only update if the walking state is not already true
            {
                isWalking.Value = true; // Set walking state to true
            }
        }
        else
        {
            if (isWalking.Value) // Only update if the walking state is not already false
            {
                isWalking.Value = false; // Set walking state to false
            }
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts.Length > 0)
        {
            isGrounded = true;
        }
    }

    [ServerRpc]
    private void UpdatePositionServerRpc(Vector2 newPosition, ServerRpcParams rpcParams = default)
    {
        Position.Value = newPosition;
    }

    private void OnPositionChanged(Vector2 oldPosition, Vector2 newPosition)
    {
        if (!IsOwner)
        {
            transform.position = new Vector3(newPosition.x, newPosition.y, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            ReturnToStart();
        }
    }

    // Function to toggle gravity and invert the direction
    private void ToggleGravity()
    {
        if (isGravityInverted)
        {
            Physics2D.gravity = originalGravity;  // Reset to normal gravity
        }
        else
        {
            Physics2D.gravity = new Vector2(0f, -originalGravity.y);  // Invert gravity direction
        }

        isGravityInverted = !isGravityInverted;  // Toggle the gravity state
    }

    // Update the walking animation based on the walking state
    private void UpdateWalkingAnimation()
    {
        if (playerAnimation != null)
        {
            if (isWalking.Value)
            {
                if (!playerAnimation.isPlaying || playerAnimation["Run"].time == 0)
                {
                    playerAnimation.Play("Run"); // Play the walk animation
                }
            }
            else
            {
                playerAnimation.Stop(); // Stop the current animation
                playerAnimation.Play("Idle"); // Play the idle animation
            }
        }
    }

    private void OnWalkingStateChanged(bool oldState, bool newState)
    {
        if (playerAnimation != null)
        {
            if (newState)
            {
                if (!playerAnimation.isPlaying || playerAnimation["Run"].time == 0)
                {
                    playerAnimation.Play("Run"); // Play the walk animation
                }
            }
            else
            {
                playerAnimation.Stop("Run"); // Stop the walk animation
            }
        }
    }
}
