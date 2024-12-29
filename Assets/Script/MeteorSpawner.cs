using System.Collections; // تأكد من إضافة هذه السطر
using UnityEngine;

public class MeteorShower : MonoBehaviour
{
    public GameObject meteorPrefab; // النيزك
    public Transform meteorSpawnArea; // المنطقة التي يتم منها إنزال النيازك
    public float meteorSpawnHeight = 10f; // ارتفاع بدء سقوط النيازك
    public float meteorSpeed = 5f; // سرعة سقوط النيازك
    public float spawnInterval = 0.5f; // الوقت بين سقوط كل نيزك (مثل المطر)
    public int maxMeteors = 10; // الحد الأقصى لعدد النيازك في وقت واحد

    private void Start()
    {
        StartCoroutine(SpawnMeteorShower()); // بدء الكوروتين لإنزال النيازك
    }

    // كوروتين لإنزال النيازك بشكل مستمر
    private IEnumerator SpawnMeteorShower()
    {
        while (true)
        {
            if (transform.childCount < maxMeteors) // تأكد من أن عدد النيازك لا يتجاوز الحد الأقصى
            {
                SpawnMeteor(); // استدعاء دالة إنزال نيزك جديد
            }

            yield return new WaitForSeconds(spawnInterval); // الانتظار بين كل نيزك وآخر
        }
    }

    // دالة لإنزال نيزك
    private void SpawnMeteor()
    {
        // تحديد موقع عشوائي للنيزك فوق منطقة السقوط
        Vector3 spawnPosition = new Vector3(
            Random.Range(meteorSpawnArea.position.x - 5f, meteorSpawnArea.position.x + 5f),
            meteorSpawnArea.position.y + meteorSpawnHeight,
            0f
        );

        // إنشاء النيزك في الموقع المحدد
        GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);

        // تحريك النيزك للأسفل
        Rigidbody2D meteorRb = meteor.GetComponent<Rigidbody2D>();
        if (meteorRb != null)
        {
            meteorRb.linearVelocity = new Vector2(0f, -meteorSpeed); // جعل النيزك يسقط للأسفل بسرعة معينة
        }

        // تدمير النيزك بعد فترة زمنية قصيرة (مثل 2 ثانية)
        Destroy(meteor, 2f); // اختفاء النيزك بعد 2 ثانية
    }
}