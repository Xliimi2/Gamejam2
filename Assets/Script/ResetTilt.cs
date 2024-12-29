using UnityEngine;

public class ResetTilt : MonoBehaviour
{
    
    private Rigidbody2D rb;
    private float resetSpeed = 10f; // سرعة استعادة الوضع الطبيعي
    private bool isPlayerOnPlatform = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // إذا لم يكن اللاعب على الأرضية، أعد زاوية الدوران إلى الوضع الأفقي (0 درجة)
        if (!isPlayerOnPlatform)
        {
            float currentAngle = rb.rotation; // زاوية الدوران الحالية
            float targetAngle = 0f; // الزاوية المستهدفة (الوضع الأفقي)
            float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.fixedDeltaTime * resetSpeed);
            rb.MoveRotation(newAngle);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // عندما يصطدم اللاعب بالأرضية
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // عندما يغادر اللاعب الأرضية
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = false;
        }
    }
}