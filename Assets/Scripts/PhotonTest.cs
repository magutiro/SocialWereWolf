using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public class PhotonTest : MonoBehaviourPunCallbacks
{
    //　シーンロード中に表示するUI画面
    [SerializeField]
    private GameObject loadUI = null;
    //　タイトルに表示するUI画面
    [SerializeField]
    private GameObject inGameUI = null;

    private GameObject playerPrefab = null;
    // Start is called before the first frame update
    void Start()
    {
        // プレイヤー自身の名前を"Player"に設定する
        PhotonNetwork.NickName = UserLoginData.userName;
        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();

        if(SceneManager.GetActiveScene().name == "InGameScene")
        {
            CreatePlayer();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        // "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        CreatePlayer();

        PhotonNetwork.AutomaticallySyncScene = true;
    }
    void CreatePlayer()
    {
        // プレイヤーのリソース(プレハブ)を取得 ※初回のみ
        playerPrefab = playerPrefab ?? (GameObject)Resources.Load("Player");
        // ランダムな座標に自身のアバター（ネットワークオブジェクト）を生成する
        var position = new Vector3(Random.Range(-2f, 2f), Random.Range(-3f, 3f));
        var player = PhotonNetwork.Instantiate("Player", position, Quaternion.identity);
        player.gameObject.tag = "Player";
        player.gameObject.transform.parent = gameObject.transform;
        //　ロード画面UIをアクティブにする
        loadUI.SetActive(false);
        //　ロード画面UIをアクティブにする
        inGameUI.SetActive(true);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetStartTime(PhotonNetwork.ServerTimestamp);
        }
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        
    }

    
}
