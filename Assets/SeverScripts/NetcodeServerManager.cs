using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class NetcodeServerManager : MonoBehaviour
{
    private string _textIpAddress = "127.0.0.1";
    private string _port = "7777";
    private string _playerName = "プレイヤー名";
    public string PlayerName
    {
        get => _playerName;
    }
    private void Awake()
    {
        Application.targetFrameRate = 60; 
    }
    private void Update()
    {
#if UNITY_SERVER
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.Log("StartServer");
            StartServer();
        }
#else
        
#endif
    }
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 150, 220));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
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
        if (GUILayout.Button("Server"))
        {
            //サーバーとして起動
            StartServer();
        }

#if UNITY_SERVER
#else
        if (GUILayout.Button("Host"))
        {
            StartHost();
        }

        if (GUILayout.Button("Client"))
        {
            //クライアントとして起動
            StartClient(_textIpAddress.Trim(), Convert.ToUInt16(_port));
        }
#endif
        /*
        GUILayout.Label("IpAddress");
        _textIpAddress = GUILayout.TextField(_textIpAddress);

        GUILayout.Label("Port");
        _port = GUILayout.TextField(_port);

        GUILayout.Label("PlayerName");
        _playerName = GUILayout.TextField(_playerName);
        */
    }
    private void StartServer()
    {
        NetworkManager.Singleton.OnServerStarted += OnStartServer;
        NetworkManager.Singleton.NetworkConfig.NetworkTransport.Initialize();

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartServer();
    }

    private void StartHost()
    {
        NetworkManager.Singleton.OnServerStarted += OnStartServer;
        NetworkManager.Singleton.NetworkConfig.NetworkTransport.Initialize();
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost();
    }

    private void StartClient(string ipAddress, ushort port)
    {
        var transport = Unity.Netcode.NetworkManager.Singleton.NetworkConfig.NetworkTransport;

        if (transport is Unity.Netcode.Transports.UNET.UNetTransport)
        {
            var unetTransport = transport as Unity.Netcode.Transports.UNET.UNetTransport;
            if (unetTransport != null)
            {
                unetTransport.ConnectAddress = ipAddress;
                unetTransport.ConnectPort = port;
            }
        }

        NetworkManager.Singleton.StartClient();
    }
    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        //Your logic here
        bool approve = true;
        bool createPlayerObject = true;

        // Position to spawn the player object at, set to null to use the default position
        Vector3? positionToSpawnAt = Vector3.zero;

        // Rotation to spawn the player object at, set to null to use the default rotation
        Quaternion rotationToSpawnWith = Quaternion.identity;

        //If approve is true, the connection gets added. If it's false. The client gets disconnected
        callback(createPlayerObject, null, approve, positionToSpawnAt, rotationToSpawnWith);
    }
    void OnStartServer()
    {

    }

    private void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ? "Host" :
            NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
                        NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }
}
