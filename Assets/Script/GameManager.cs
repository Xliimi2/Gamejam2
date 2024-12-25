using UnityEngine;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
    public GameObject[] playerPrefabs; // قائمة Prefabs لكل لاعب

    private NetworkManager m_NetworkManager;

    void Awake()
    {
        // التأكد من تعيين NetworkManager
        m_NetworkManager = GetComponent<NetworkManager>();

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
            m_NetworkManager.OnClientConnectedCallback += OnClientConnected;

            // بدء المضيف تلقائيًا للتجربة
            m_NetworkManager.StartHost();
        }
    }

    private void OnGUI()
    {
        if (m_NetworkManager == null)
        {
            GUILayout.Label("NetworkManager is not available!");
            return;
        }

        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        if (!m_NetworkManager.IsClient && !m_NetworkManager.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        GUILayout.EndArea();
    }

    private void StartButtons()
    {
        if (GUILayout.Button("Host")) 
        {
            m_NetworkManager.StartHost();
        }
        if (GUILayout.Button("Client"))
        {
            m_NetworkManager.StartClient();
        }
        if (GUILayout.Button("Server"))
        {
            m_NetworkManager.StartServer();
        }
    }

    private void StatusLabels()
    {
        var mode = m_NetworkManager.IsHost ? "Host" : m_NetworkManager.IsServer ? "Server" : "Client";
        GUILayout.Label("Transport: " + m_NetworkManager.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }

    private void OnClientConnected(ulong clientId)
    {
        if (m_NetworkManager.IsServer)
        {
            // اختيار Prefab بناءً على ID العميل
            int prefabIndex = (int)(clientId % (ulong)playerPrefabs.Length);
            GameObject selectedPrefab = playerPrefabs[prefabIndex];

            // إنشاء الكائن
            GameObject playerInstance = Instantiate(selectedPrefab);

            // التأكد من أن الكائن يحتوي على NetworkObject
            NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                // تعيين الملكية للعميل
                networkObject.SpawnWithOwnership(clientId);
            }
            else
            {
                Debug.LogError("Player Prefab is missing a NetworkObject component!");
            }
        }
    }
}
