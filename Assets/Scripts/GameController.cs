using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameState
{
    public enum gameState
    {
        Room = 0,
    } 
}
public class GameController : MonoBehaviour
{
    float _gameTime = 300;
    [SerializeField]
    Text _timeText;

    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "InGameScene") { }
            //_timeText = GameObject.Find("TimeText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        // まだルームに参加していない場合は更新しない
        if (!PhotonNetwork.InRoom) { return; }
        // まだゲームの開始時刻が設定されていない場合は更新しない
        if (!PhotonNetwork.CurrentRoom.TryGetStartTime(out int timestamp)) { return; }

        // ゲームの経過時間を求めて、小数第一位まで表示する
        float elapsedTime = Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - timestamp) / 1000f);
        elapsedTime = _gameTime - elapsedTime;
        _timeText.text = Mathf.FloorToInt(elapsedTime / 60) +":"+ (elapsedTime % 60).ToString("f1");
        */
    }

    
}
