using UnityEngine;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
    public GameObject[] playerPrefabs; 
    public GameObject monsterPrefab; 

    private NetworkManager m_NetworkManager;

    void Awake()
    {
        m_NetworkManager = GetComponent<NetworkManager>();

        if (m_NetworkManager == null)
        {
            Debug.LogError("NetworkManager is missing from the GameManager object!");
        }
    }

    void Start()
    {
        if (m_NetworkManager != null)
        {
            m_NetworkManager.OnClientConnectedCallback += OnClientConnected;

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
            int prefabIndex = (int)(clientId % (ulong)playerPrefabs.Length);
            GameObject playerInstance = Instantiate(playerPrefabs[prefabIndex]);
            NetworkObject playerNetworkObject = playerInstance.GetComponent<NetworkObject>();
            if (playerNetworkObject != null)
            {
                playerNetworkObject.SpawnWithOwnership(clientId);
            }

            GameObject monsterInstance = Instantiate(monsterPrefab);
            NetworkObject monsterNetworkObject = monsterInstance.GetComponent<NetworkObject>();
            if (monsterNetworkObject != null)
            {
                monsterNetworkObject.SpawnWithOwnership(clientId);
            }
        }
    }


}
