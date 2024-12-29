using UnityEngine;
using Unity.Netcode;

public class Monster : NetworkBehaviour
{
    private int hitCount = 0; // عدد الطلقات التي أصابت الوحش
    private Collider2D monsterCollider;

   
    public void TakeDamage()
    {
        if (IsOwner)
        {
            hitCount++;
            if (hitCount >= 3)
            {
            }
        }
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