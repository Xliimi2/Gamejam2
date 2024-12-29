using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class TestRelay : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private string relayCode;

    public TMP_InputField inputText;
    
    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($" signed in {AuthenticationService.Instance.PlayerId}");
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateLobby()
    {
        try
        {
           Allocation allocation =  await RelayService.Instance.CreateAllocationAsync(2);

           relayCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

           inputText.text = relayCode;
           NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
               allocation.RelayServer.IpV4,
               (ushort)allocation.RelayServer.Port,
               allocation.AllocationIdBytes,
               allocation.Key,
               allocation.ConnectionData
               );

               Debug.Log(relayCode);

               NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void JoinLobby()
    {

        try
        {
            relayCode = inputText.text;
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayCode);
        
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            UnityEngine.Debug.LogError(e);
            
        }
        
    }

    // Update is called once per frame
}