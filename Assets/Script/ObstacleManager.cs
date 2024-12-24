using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public GameObject[] obstacles; // مصفوفة تحتوي على جميع العوائق

    private void Start()
    {
        // ضبط جميع العوائق كـ Triggers في حالة لعبة 2D
        foreach (GameObject obstacle in obstacles)
        {
            Collider2D obstacleCollider = obstacle.GetComponent<Collider2D>(); // استخدم Collider2D
            if (obstacleCollider != null)
            {
                if (!obstacleCollider.isTrigger)
                {
                    obstacleCollider.isTrigger = true; // تعيين IsTrigger إلى true إذا كنت تريد تفاعل تحفيز
                }
            }
            else
            {
                Debug.LogWarning("No Collider2D found on obstacle: " + obstacle.name);
            }
        }
    }

    // التعامل مع الاصطدام بين اللاعب والعائق
    private void OnTriggerEnter2D(Collider2D other)
    {
        // تحقق إذا كان الكائن الذي اصطدم هو اللاعب
        if (other.CompareTag("Player"))
        {
            PlayerMovementTest player = other.GetComponent<PlayerMovementTest>();
            if (player != null)
            {
                Debug.Log("Player hit an obstacle. Returning to start.");
                player.ReturnToStart(); // إعادة اللاعب إلى بداية اللعبة
            }
            else
            {
                Debug.LogWarning("No PlayerMovementTest component found on the player.");
            }
        }
    }
}