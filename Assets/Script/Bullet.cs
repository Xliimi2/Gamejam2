using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster")) // تحقق إذا كان الكائن هو وحش
        {
            Monster monster = other.GetComponent<Monster>();
            if (monster != null)
            {
                monster.TakeDamage(); // قم بإبلاغ الوحش بأنه قد تلقى ضربة
            }
            Destroy(gameObject); // تدمير الرصاصة بعد الاصطدام
        }
    }
}