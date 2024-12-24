using UnityEngine;
using Unity.Netcode;

public class PlayerMovementTest : NetworkBehaviour
{
    // متغير شبكة لمزامنة المواقع
    public NetworkVariable<Vector2> Position = new NetworkVariable<Vector2>(
        new Vector2(0f, 0f), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public float moveSpeed = 5f;  // سرعة الحركة
    public float jumpForce = 10f; // قوة القفز
    private Rigidbody2D rb;       // المكون الفيزيائي للحركة
    private bool isGrounded = true;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // مراقبة التغييرات في الموقع
        Position.OnValueChanged += OnPositionChanged;
    }

    void Update()
    {
        if (IsOwner)
        {
            Move();  // التحكم في الحركة والقفز

            // إرسال الموقع إلى الخادم إذا تغير
            if (Vector2.Distance(Position.Value, rb.position) > 0.1f)
            {
                UpdatePositionServerRpc(rb.position);
            }
        }
        else
        {
            // تحديث الموقع بناءً على القيمة المتزامنة
            transform.position = new Vector3(Position.Value.x, Position.Value.y, 0f);
        }
    }

    public void Move()
    {
        HandleMovement();  // التحكم في الحركة
        HandleJump();      // التحكم في القفز
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = movement;
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
        // التحقق من إذا كان اللاعب على الأرض
        if (collision.contacts.Length > 0)
        {
            isGrounded = true;
        }
    }

    [ServerRpc]
    private void UpdatePositionServerRpc(Vector2 newPosition, ServerRpcParams rpcParams = default)
    {
        // تحديث الموقع في الخادم
        Position.Value = newPosition;
    }

    private void OnPositionChanged(Vector2 oldPosition, Vector2 newPosition)
    {
        if (!IsOwner)
        {
            // تحديث الموقع للعملاء الآخرين
            transform.position = new Vector3(newPosition.x, newPosition.y, 0f);
        }
    }
}
