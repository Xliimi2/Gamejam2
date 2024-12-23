using UnityEngine;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
    private NetworkManager m_NetworkManager;

    void Awake()
    {
        // التأكد من تعيين NetworkManager
        m_NetworkManager = GetComponent<NetworkManager>();

        // التحقق من وجود NetworkManager في الكائن
        if (m_NetworkManager == null)
        {
            Debug.LogError("NetworkManager is missing from the GameManager object!");
        }
    }

    void Start()
    {
        // إذا كان m_NetworkManager غير مُعين بشكل صحيح، لا نريد أن نستمر في هذه الدالة
        if (m_NetworkManager != null)
        {
            m_NetworkManager.StartHost();  // محاولة بدء مضيف اللعبة (Host)
        }
    }

    void OnGUI()
    {
        // التحقق إذا كان m_NetworkManager غير null
        if (m_NetworkManager == null)
        {
            GUILayout.Label("NetworkManager is not available!");
            return; // إذا لم يكن المكون موجودًا، نوقف تنفيذ باقي الدالة
        }

        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        // إذا لم يكن العميل أو الخادم قيد التشغيل، عرض الأزرار
        if (!m_NetworkManager.IsClient && !m_NetworkManager.IsServer)
        {
            StartButtons();
        }
        else
        {
            // عرض حالة الاتصال
            StatusLabels();
            SubmitNewPosition();
        }

        GUILayout.EndArea();
    }

    private void StartButtons()
    {
        if (GUILayout.Button("Host")) 
        {
            m_NetworkManager.StartHost();  // بدء المضيف
        }
        if (GUILayout.Button("Client"))
        {
            m_NetworkManager.StartClient();  // بدء العميل
        }
        if (GUILayout.Button("Server"))
        {
            m_NetworkManager.StartServer();  // بدء الخادم
        }
    }

    private void StatusLabels()
    {
        var mode = m_NetworkManager.IsHost ?
            "Host" : m_NetworkManager.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " + 
                        m_NetworkManager.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }

    private void SubmitNewPosition()
    {
        if (GUILayout.Button(m_NetworkManager.IsServer ? "Move" : "Request Position Change"))
        {
            // إذا كان الخادم، نقوم بتحريك اللاعبين
            if (m_NetworkManager.IsServer && !m_NetworkManager.IsClient)
            {
                foreach (ulong uid in m_NetworkManager.ConnectedClientsIds)
                {
                    var playerNetworkObject = m_NetworkManager.SpawnManager.GetPlayerNetworkObject(uid);
                    var playerMovement = playerNetworkObject.GetComponent<PlayerMovementTest>();
                    playerMovement.Move();  // تنفيذ الحركة
                }
            }
            else
            {
                // إذا كان العميل، نقوم بطلب تغيير الوضع
                var playerObject = m_NetworkManager.SpawnManager.GetLocalPlayerObject();
                var player = playerObject.GetComponent<PlayerMovementTest>();
                player.Move();  // تنفيذ الحركة
            }
        }
    }
}
