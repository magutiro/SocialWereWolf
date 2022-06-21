using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using UnityEngine.Networking.Match;

public class NetcodeServerManager : MonoBehaviour
{
    //private string _textIpAddress = "54.168.7.191";
    //private string _port = "1935";

    private string _textIpAddress = "127.0.0.1";//127.0.0.1
    private string _port = "7777";

    private string _playerName = "�v���C���[��";


    private ConnectInfo connectInfo;
    public GameLift gameLift;

    NetworkSpawnManager NetworkSpawnManager;
    string inURL = "https://84k5pqnqqf.execute-api.ap-northeast-1.amazonaws.com/default/test";
    public string PlayerName
    {
        get => _playerName;
    }
    private void Awake()
    {
        connectInfo = new ConnectInfo();
        Application.targetFrameRate = 60;
    }
    private void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

#if SERVER
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.Log("StartServer");
            StartServer();
        }

#endif
    }
    private void Update()
    {
    }

#if CLIENT
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

#endif
    private void StartButtons()
    {

#if UNITY_SERVER
#elif CLIENT     
        if (GUILayout.Button("Server"))
        {
            //�T�[�o�[�Ƃ��ċN��
            StartServer();
        }

        if (GUILayout.Button("Host"))
        {
            StartHost();
        }

        if (GUILayout.Button("Client"))
        {
            //�N���C�A���g�Ƃ��ċN��
            StartClient(_textIpAddress.Trim(), Convert.ToUInt16(_port));
        }
        if (GUILayout.Button("GameLiftClient"))
        {
            //�N���C�A���g�Ƃ��ċN��
            OnClickGameLiftConnect();
        }
#endif
    }//�N���C�A���g�̐ڑ������F����H

    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        Debug.Log("ApprovalCheck connectionData:" + System.Text.Encoding.ASCII.GetString(connectionData));
        //Your logic here
        //bool createPlayerObject = true;
        bool approve;
        callback(false, null, true, null, null);

#if SERVER
            if (gameLift == null)
            {
                Debug.Log("GameLift object is null!");
            }
            else
            {
                //GameLift�Ƀv���C���[�Z�b�V������₢���킹��
                approve = gameLift.ConnectPlayer((int)clientId, System.Text.Encoding.ASCII.GetString(connectionData));
                if (!approve) { DisconnectPlayer(clientId); }
            }
#endif


        // The prefab hash. Use null to use the default player prefab
        // �v���n�u�n�b�V���B�f�t�H���g�̃v���[���[�v���n�u���g�p����ɂ́Anull���g�p���܂�
        // If using this hash, replace "MyPrefabHashGenerator" with the name of a prefab added to the NetworkedPrefabs field of your NetworkingManager object in the scene
        // ���̃n�b�V�����g�p����ꍇ�́A�uMyPrefabHashGenerator�v���A�V�[������NetworkingManager�I�u�W�F
        // �N�g��NetworkedPrefabs�t�B�[���h�ɒǉ����ꂽ�v���n�u�̖��O�ɒu�������܂��B
        //ulong? prefabHash = SpawnManager.GetPrefabHashFromGenerator("MyPrefabHashGenerator");

        //If approve is true, the connection gets added. If it's false. The client gets disconnected
        // ���F��true�̏ꍇ�A�ڑ����ǉ�����܂��B���ꂪ�Ԉ���Ă���ꍇ�B�N���C�A���g���ؒf��
        // ��܂�
        //callback(createPlayerObject, prefabHash, approve, positionToSpawnAt, rotationToSpawnWith);
        //callback(false, null, approve, null, null);
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
        //var clientId = NetworkManager.Singleton.LocalClientId;

       //SpawnCharacterServerRpc(clientId);
    }
    public void OnClickGameLiftConnect()
    {
        Debug.Log("GMLIFT");
#if CLIENT
        // try to connect to gamelift
        StartCoroutine(CallGetWebRequest(inURL));
        
#endif
    }
    private IEnumerator CallGetWebRequest(string inURL)
    {
        //�E�F�u���N�G�X�g�𐶐�
        var request = UnityEngine.Networking.UnityWebRequest.Get(inURL);
        //�ʐM�҂�
        yield return request.SendWebRequest();
        Debug.Log(request); 
        string text;
        yield return text = request.downloadHandler.text; 
        //Lambda req;
        //yield return req = JsonUtility.FromJson<Lambda>(text);
        Debug.Log(text.ToString());
        //yield return text = req.body.Replace("\\", "");
        body body;
        yield return body = JsonUtility.FromJson<body>(text);

        this.connectInfo.ipAddr = body.IpAddress;
        this.connectInfo.port = body.Port;
        this.connectInfo.playerName = UserLoginData.userName.Value;
        ApplyConnectInfoToNetworkManager();
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(body.PlayerSessionId);
        var tasks = NetworkManager.Singleton.StartClient();
    }
    // �ڑ��ݒ��MLAPI�̃l�b�g���[�N�ݒ�ɔ��f�����܂�
    private void ApplyConnectInfoToNetworkManager()
    {
        // NetworkManager����ʐM���̂�Transport���擾���܂�
        var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport;

        // ��UnetTransport�Ƃ��Ĉ����܂�
        var unetTransport = transport as Unity.Netcode.Transports.UNET.UNetTransport;
        if (unetTransport != null)
        {
            // relay�T�[�o�[�g�p���邩�H
            //unetTransport. = this.connectInfo.useRelay;

            if (this.connectInfo.useRelay)
            {
                unetTransport.ConnectAddress = this.connectInfo.relayIpAddr.Trim();
                unetTransport.ConnectPort = this.connectInfo.relayPort;
            }
            // �ڑ���A�h���X�w��(Client��)
            unetTransport.ConnectAddress = this.connectInfo.ipAddr.Trim();
            // �ڑ��|�[�g�ԍ��w��
            unetTransport.ConnectPort = this.connectInfo.port;
            // �T�[�o�[���ł̃|�[�g�w��
            unetTransport.ServerListenPort = this.connectInfo.port;
        }

    }
    private void OnClientDisconnect(ulong clientId)
    {
        Debug.Log("Disconnect Client " + clientId);
        DisconnectPlayer(clientId);
    }

    //GameLift����v���C���[�Z�b�V�������폜
    private void DisconnectPlayer(ulong clientId)
    {
#if SERVER
            gameLift.DisconnectPlayer((int)clientId);   //�v���C���[�Z�b�V���������
#endif
    }
    void OnStartServer()
    {
        Debug.Log("Plyaer�X�|�[��");
        var clientId = NetworkManager.Singleton.LocalClientId;
        // host�Ȃ�ΐ������܂�
        if (NetworkManager.Singleton.IsClient)
        {
            //SpawnCharacterServerRpc(clientId);
        }
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
