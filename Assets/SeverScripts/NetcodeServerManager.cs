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

    private string _playerName = "プレイヤー名";


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
            //サーバーとして起動
            StartServer();
        }

        if (GUILayout.Button("Host"))
        {
            StartHost();
        }

        if (GUILayout.Button("Client"))
        {
            //クライアントとして起動
            StartClient(_textIpAddress.Trim(), Convert.ToUInt16(_port));
        }
        if (GUILayout.Button("GameLiftClient"))
        {
            //クライアントとして起動
            OnClickGameLiftConnect();
        }
#endif
    }//クライアントの接続を承認する？

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
                //GameLiftにプレイヤーセッションを問い合わせる
                approve = gameLift.ConnectPlayer((int)clientId, System.Text.Encoding.ASCII.GetString(connectionData));
                if (!approve) { DisconnectPlayer(clientId); }
            }
#endif


        // The prefab hash. Use null to use the default player prefab
        // プレハブハッシュ。デフォルトのプレーヤープレハブを使用するには、nullを使用します
        // If using this hash, replace "MyPrefabHashGenerator" with the name of a prefab added to the NetworkedPrefabs field of your NetworkingManager object in the scene
        // このハッシュを使用する場合は、「MyPrefabHashGenerator」を、シーン内のNetworkingManagerオブジェ
        // クトのNetworkedPrefabsフィールドに追加されたプレハブの名前に置き換えます。
        //ulong? prefabHash = SpawnManager.GetPrefabHashFromGenerator("MyPrefabHashGenerator");

        //If approve is true, the connection gets added. If it's false. The client gets disconnected
        // 承認がtrueの場合、接続が追加されます。それが間違っている場合。クライアントが切断さ
        // れます
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
        //ウェブリクエストを生成
        var request = UnityEngine.Networking.UnityWebRequest.Get(inURL);
        //通信待ち
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
    // 接続設定をMLAPIのネットワーク設定に反映させます
    private void ApplyConnectInfoToNetworkManager()
    {
        // NetworkManagerから通信実体のTransportを取得します
        var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport;

        // ※UnetTransportとして扱います
        var unetTransport = transport as Unity.Netcode.Transports.UNET.UNetTransport;
        if (unetTransport != null)
        {
            // relayサーバー使用するか？
            //unetTransport. = this.connectInfo.useRelay;

            if (this.connectInfo.useRelay)
            {
                unetTransport.ConnectAddress = this.connectInfo.relayIpAddr.Trim();
                unetTransport.ConnectPort = this.connectInfo.relayPort;
            }
            // 接続先アドレス指定(Client時)
            unetTransport.ConnectAddress = this.connectInfo.ipAddr.Trim();
            // 接続ポート番号指定
            unetTransport.ConnectPort = this.connectInfo.port;
            // サーバー側でのポート指定
            unetTransport.ServerListenPort = this.connectInfo.port;
        }

    }
    private void OnClientDisconnect(ulong clientId)
    {
        Debug.Log("Disconnect Client " + clientId);
        DisconnectPlayer(clientId);
    }

    //GameLiftからプレイヤーセッションを削除
    private void DisconnectPlayer(ulong clientId)
    {
#if SERVER
            gameLift.DisconnectPlayer((int)clientId);   //プレイヤーセッションを解放
#endif
    }
    void OnStartServer()
    {
        Debug.Log("Plyaerスポーン");
        var clientId = NetworkManager.Singleton.LocalClientId;
        // hostならば生成します
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
