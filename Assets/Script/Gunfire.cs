using UnityEngine;

public class GunFire : MonoBehaviour
{
    public GameObject bulletPrefab; // Prefab للرصاصة
    public Transform firePoint;     // نقطة إطلاق النار
    public float bulletSpeed = 10f; // سرعة الرصاصة

    // لا حاجة لتعريف private هنا بشكل غير صحيح
    void Update()
    {
        // التحقق من الضغط على الزر لإطلاق النار
        if (Input.GetMouseButtonDown(0)) // الزر الأيسر للفأرة
        {
            FireBullet();
        }
    }

    // الدالة الخاصة بإطلاق الرصاصة
    void FireBullet()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // إنشاء الرصاصة من الـ Prefab
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // إضافة قوة للرصاصة لتحركها للأمام
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = firePoint.right * bulletSpeed;
            }
        }
    }
}