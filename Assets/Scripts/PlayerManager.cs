using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private GameObject playerPrefab = null;     // プレイヤーのリソース(プレハブ)
    private GameObject player;                  // 自プレイヤー情報
    private const float KEY_MOVEMENT = 0.5f;    // 移動ボタン1回クリックでの移動量

    // 全プレイヤーの行動情報
    private Dictionary<string, PlayerActionData> PlayerActionMap;

    // 全プレイヤーのオブジェクト情報
    private readonly Dictionary<string, GameObject> playerObjectMap = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // 自プレイヤーの作成
        player = MakePlayer(new Vector3(-3, 3, 0), UserLoginData.userName);
        // WebSocket開始
        StartWebSocket();
    }
    void Update()
    {
        // ユーザーの行動情報があったら同期処理を行い、ユーザーの行動情報を初期化
        if (PlayerActionMap != null)
        {
            Synchronaize();
            PlayerActionMap = null;
        }
    }
    /// <summary>
    /// WebSocketの開始
    /// </summary>
    private void StartWebSocket()
    {
        // WebSocket通信開始
        WebSocketClientManager.Connect();

        // WebSocketのメッセージ受信メソッドの設定
        WebSocketClientManager.recieveCompletedHandler += OnReciveMessage;

        // 自プレイヤーの初期情報をWebSocketに送信
        WebSocketClientManager.SendPlayerAction("connect", new Vector3(-3, 3, 0), "neutral", 0.0f);
    }

    /// <summary>
    /// WebSocketの終了
    /// </summary>
    private void EndWebsocket()
    {
        WebSocketClientManager.SendPlayerAction("disconnect", Vector3.zero, "neutral", 0.0f);
        WebSocketClientManager.DisConnect();
    }
    private void OnReciveMessage(Dictionary<string, PlayerActionData> PlayerActionMap)
    {
        // 同期情報を取得
        this.PlayerActionMap = PlayerActionMap;
    }

    /// <summary>
    /// 同期処理
    /// </summary>
    private void Synchronaize()
    {

        // 退出した他プレイヤーの検索
        List<string> otherPlayerNameList = new List<string>(playerObjectMap.Keys);
        foreach (var otherPlayerName in otherPlayerNameList)
        {
            // 退出したプレイヤーの削除
            if (!PlayerActionMap.ContainsKey(otherPlayerName))
            {
                Destroy(playerObjectMap[otherPlayerName]);
                playerObjectMap.Remove(otherPlayerName);
            }
        }

        // プレイヤーの位置を更新
        foreach (var playerAction in PlayerActionMap.Values)
        {
            // 自分は移動済みなのでスルー
            if (UserLoginData.userName == playerAction.user)
            {
                continue;
            }

            // 入室中の他プレイヤーの移動
            if (playerObjectMap.ContainsKey(playerAction.user))
            {
                playerObjectMap[playerAction.user].transform.position = GetMovePos(playerAction);

                // 入室中した他プレイヤーの生成
            }
            else
            {
                // 他プレイヤーの作成
                Debug.Log("play2");
                var player = MakePlayer(GetMovePos(playerAction), playerAction.user);

                // 他プレイヤーリストへの追加
                playerObjectMap.Add(playerAction.user, player);
            }
        }
    }
    /// <summary>
    /// 上ボタン押下時の処理
    /// </summary>
    /*
    public void OnClickUpButton()
    {
        player.transform.Translate(0, 0, KEY_MOVEMENT);
    }

    /// <summary>
    /// 下ボタン押下時の処理
    /// </summary>
    public void OnClickDownButton()
    {
        player.transform.Translate(0, 0, -1 * KEY_MOVEMENT);
    }

    /// <summary>
    /// 左ボタン押下時の処理
    /// </summary>
    public void OnClickLeftButton()
    {
        player.transform.Translate(-1 * KEY_MOVEMENT, 0, 0);
    }

    /// <summary>
    /// 右ボタン押下時の処理
    /// </summary>
    public void OnClickRightButton()
    {
        player.transform.Translate(KEY_MOVEMENT, 0, 0);
    }

    /// <summary>
    /// 退室ボタン押下時の処理
    /// </summary>
    public void OnClickExitButton()
    {
        // タイトルシーンに戻る
        SceneManager.LoadScene("TitleScene");
    }*/

    /// <summary>
    /// プレイヤーを作成
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="name"></param>
    private GameObject MakePlayer(Vector3 pos, string name)
    {
        // プレイヤーのリソース(プレハブ)を取得 ※初回のみ
        playerPrefab = playerPrefab ?? (GameObject)Resources.Load("Player");

        // プレイヤーを生成
        var player = (GameObject)Instantiate(playerPrefab, pos, Quaternion.identity);

        // プレイヤーのネームプレートの設定
        var otherNameText = player.transform.Find("Name").gameObject;
        otherNameText.GetComponent<TextMesh>().text = name;

        return player;
    }
    private Vector3 GetMovePos(PlayerActionData playerAction)
    {
        var pos = new Vector3(playerAction.pos_x, playerAction.pos_y, playerAction.pos_z);
        pos.y += (playerAction.way == "up") ? playerAction.range : 0;
        pos.y -= (playerAction.way == "down") ? playerAction.range : 0;
        pos.x -= (playerAction.way == "left") ? playerAction.range : 0;
        pos.x += (playerAction.way == "right") ? playerAction.range : 0;

        return pos;
    }
}
