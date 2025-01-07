using UnityEngine;
using Unity.Netcode;

public class Monster : NetworkBehaviour
{
    private int hitCount = 0; // عدد الطلقات التي أصابت الوحش
    private Collider2D monsterCollider;

    void Start()
    {
        monsterCollider = GetComponent<Collider2D>();

        if (!IsOwner)
        {
            gameObject.SetActive(true); // إخفاء الوحش عن اللاعبين الآخرين
        }
    }

    public void TakeDamage()
    {
        if (IsOwner)
        {
            hitCount++;
            if (hitCount >= 3)
            {
                DestroyMonsterServerRpc(); // إزالة الوحش بعد ثلاث طلقات
            }
        }
    }

    [ServerRpc]
    private void DestroyMonsterServerRpc()
    {
        DestroyMonsterClientRpc(); // مزامنة الإزالة مع جميع اللاعبين
        Destroy(gameObject);       // إزالة الوحش من السيرفر
    }

    [ClientRpc]
    private void DestroyMonsterClientRpc()
    {
        if (gameObject != null)
        {
            Destroy(gameObject); // إزالة الوحش على جميع الأجهزة
        }
    }
}