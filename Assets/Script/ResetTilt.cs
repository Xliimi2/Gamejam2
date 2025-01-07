using UnityEngine;

public class ResetTilt : MonoBehaviour
{
    private Rigidbody2D rb;
    private float resetSpeed = 10f;
    private bool shouldReset = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (shouldReset)
        {
            float currentAngle = rb.rotation; 
            float targetAngle = 0f; 
            float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.fixedDeltaTime * resetSpeed);
            rb.MoveRotation(newAngle);

            if (Mathf.Abs(currentAngle - targetAngle) < 0.1f)
            {
                shouldReset = false;
                rb.angularVelocity = 0f; 
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            shouldReset = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            shouldReset = true; 
        }
    }
}