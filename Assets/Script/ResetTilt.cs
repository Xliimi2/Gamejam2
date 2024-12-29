using UnityEngine;

public class ResetTilt : MonoBehaviour
{
    private Rigidbody2D rb;
    private float resetSpeed = 10f; // سرعة استعادة الوضع الطبيعي
    private bool shouldReset = false; // لتحديد ما إذا كان يجب إعادة الأرضية

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // إذا كان يجب إعادة الأرضية إلى الوضع الأفقي
        if (shouldReset)
        {
            float currentAngle = rb.rotation; // زاوية الدوران الحالية
            float targetAngle = 0f; // الزاوية المستهدفة (الوضع الأفقي)
            float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.fixedDeltaTime * resetSpeed);
            rb.MoveRotation(newAngle);

            // إذا وصلت الأرضية إلى الوضع الأفقي، أوقف الإعادة
            if (Mathf.Abs(currentAngle - targetAngle) < 0.1f)
            {
                shouldReset = false;
                rb.angularVelocity = 0f; // إيقاف أي حركة زاوية
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // عندما يصطدم اللاعب بالأرضية
        if (collision.gameObject.CompareTag("Player"))
        {
            shouldReset = false; // أوقف الإعادة لأن اللاعب على الأرضية
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // عندما يغادر اللاعب الأرضية
        if (collision.gameObject.CompareTag("Player"))
        {
            shouldReset = true; // ابدأ إعادة الأرضية إلى الوضع الأفقي
        }
    }
}