using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class ServerMessage : MonoBehaviour
{
    private string _textIpAddress = "127.0.0.1";
    private string _port = "7777";
    private string _playerName = "ƒvƒŒƒCƒ„[–¼";
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
#endif
    }

    private void StartButtons()
    {
        if (GUILayout.Button("Server"))
        {
            StartServer();
        }

        if (GUILayout.Button("Host"))
        {
            StartHost();
        }

        if (GUILayout.Button("Client"))
        {
            StartClient(_textIpAddress.Trim(), Convert.ToUInt16(_port));
        }

        GUILayout.Label("IpAddress");
        _textIpAddress = GUILayout.TextField(_textIpAddress);

        GUILayout.Label("Port");
        _port = GUILayout.TextField(_port);

        GUILayout.Label("PlayerName");
        _playerName = GUILayout.TextField(_playerName);

    }
    private void StartServer()
    {
        NetworkManager.Singleton.OnServerStarted += OnStartServer;
        NetworkManager.Singleton.NetworkConfig.NetworkTransport.Initialize();
        NetworkManager.Singleton.StartServer();
    }

    private void StartHost()
    {
        NetworkManager.Singleton.OnServerStarted += OnStartServer;
        NetworkManager.Singleton.NetworkConfig.NetworkTransport.Initialize();
        NetworkManager.Singleton.StartHost();
    }

    private void StartClient(string ipAddress, ushort port)
    {
        var transport = Unity.Netcode.NetworkManager.Singleton.NetworkConfig.NetworkTransport;

        /*
        if (transport is Unity.Netcode.UnityTransport)
        {
            var unityTransport = transport as Unity.Netcode.UnityTransport;
            if (unityTransport != null)
            {
                unityTransport.SetConnectionData(ipAddress, port);
            }
        }
        */
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
    void OnStartServer()
    {

    }
}
