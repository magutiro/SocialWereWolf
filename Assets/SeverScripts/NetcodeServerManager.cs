using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using UnityEngine.Networking.Match;

public class NetcodeServerManager : MonoBehaviour
{
    private string _textIpAddress = "127.0.0.1";
    private string _port = "7777";
    private string _playerName = "プレイヤー名";


    private ConnectInfo connectInfo;
    public GameLift gameLift;
    public string PlayerName
    {
        get => _playerName;
    }
    private void Awake()
    {
        connectInfo = new ConnectInfo();
        Application.targetFrameRate = 60;
        //NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
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
        if (GUILayout.Button("Server"))
        {
            //サーバーとして起動
            StartServer();
        }

#if UNITY_SERVER
#elif CLIENT
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
        // ここにあなたの論理
        bool approve = true;
        //bool createPlayerObject = true;


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
        callback(false, null, approve, null, null);
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
    }
    public void OnClickGameLiftConnect()
    {

#if CLIENT
        // try to connect to gamelift
        //ローカルサーバが無ければGameLiftに接続する
        if (gameLift.client != null)
        {
            string ip = null;
            int port = -1;
            string auth = null;
            gameLift.GetConnectionInfo(ref ip, ref port, ref auth); // sets GameliftStatus

            if (gameLift.gameliftStatus)// TryConnect(ip, port, auth); //GameLiftからip、ポート、認証をゲットしたので接続
            {
                Debug.Log("GameLiftからIP取得！ ip:" + ip + " port:" + port + " auth:" + auth);
                //this.connectInfo.useRelay = false;
                this.connectInfo.ipAddr = ip;
                this.connectInfo.port = port;
                this.connectInfo.playerName = UserLoginData.userName;

                ApplyConnectInfoToNetworkManager();
                Debug.Log(ip);
                NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(auth); //セッションＩＤをカスタムデータとして送信

                // ClientManagerでMLAPIのコールバック等を設定
                //this.clientManager.Setup();
                // MLAPIでクライアントとして起動
                var tasks = NetworkManager.Singleton.StartClient();
                //this.clientManager.SetSocketTasks(tasks);
            }
        }
#elif !GAMELIFT
        Debug.Log("GAMELIFTのプリプロセッサ定義がありません");
#endif
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
        var clientId = NetworkManager.Singleton.LocalClientId;
        // hostならば生成します
        if (NetworkManager.Singleton.IsHost)
        {
            SpawnCharacter(clientId);
        }
    }
    private void SpawnCharacter(ulong clientId)
    {
        var netMgr = NetworkManager.Singleton;
        var networkedPrefab = netMgr.NetworkConfig.PlayerPrefab;
        var randomPosition = new Vector3(UnityEngine.Random.Range(-7, 7), 5.0f, UnityEngine.Random.Range(-7, 7));
        var gmo = GameObject.Instantiate(networkedPrefab, randomPosition, Quaternion.identity);
        var netObject = gmo.GetComponent<NetworkObject>();
        // このNetworkオブジェクトをクライアントでもSpawnさせます
        netObject.SpawnWithOwnership(clientId);
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
