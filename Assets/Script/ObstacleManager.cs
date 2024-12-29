using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public GameObject[] obstacles;

    private void Start()
    {
        foreach (GameObject obstacle in obstacles)
        {
            Collider2D obstacleCollider = obstacle.GetComponent<Collider2D>();
            if (obstacleCollider != null)
            {
                if (!obstacleCollider.isTrigger)
                {
                    obstacleCollider.isTrigger = true;
                }
            }
            else
            {
                Debug.LogWarning("No Collider2D found on obstacle: " + obstacle.name);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // تحقق إذا كان الكائن الذي اصطدم هو اللاعب
        if (other.CompareTag("Player"))
        {
            PlayerMovementTest player = other.GetComponent<PlayerMovementTest>();
            if (player != null)
            {
                Debug.Log("Player hit an obstacle. Returning to start.");
                player.ReturnToStart();
            }
            else
            {
                Debug.LogWarning("No PlayerMovementTest component found on the player.");
            }
        }
    }
}