using UnityEngine;
using Unity.Netcode; 
using System;

public class PlayerMovementTest : NetworkBehaviour
{
    public NetworkVariable<Vector2> Position = new NetworkVariable<Vector2>();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Move();
        }
    }

    public void Move()
    {
        SubmitPositionRequestRpc();
    }

    [Rpc(SendTo.Server)]
    void SubmitPositionRequestRpc(RpcParams rpcParams = default)
    {
        var randomPosition = GetRandomPositionOnPlane();
        transform.position = new Vector3(randomPosition.x, randomPosition.y, 0f); // تجاهل Z
        Position.Value = randomPosition;
    }

    static Vector2 GetRandomPositionOnPlane()
    {
        return new Vector2(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-3f, 3f)); // استخدم X و Y فقط
    }

    void Update()
    {
        // تحديث موقع اللاعب بناءً على قيمة Position
        transform.position = new Vector3(Position.Value.x, Position.Value.y, 0f); // تأكد من أن Z تظل 0
    }
}