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
        // �܂����[���ɎQ�����Ă��Ȃ��ꍇ�͍X�V���Ȃ�
        if (!PhotonNetwork.InRoom) { return; }
        // �܂��Q�[���̊J�n�������ݒ肳��Ă��Ȃ��ꍇ�͍X�V���Ȃ�
        if (!PhotonNetwork.CurrentRoom.TryGetStartTime(out int timestamp)) { return; }

        // �Q�[���̌o�ߎ��Ԃ����߂āA�������ʂ܂ŕ\������
        float elapsedTime = Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - timestamp) / 1000f);
        elapsedTime = _gameTime - elapsedTime;
        _timeText.text = Mathf.FloorToInt(elapsedTime / 60) +":"+ (elapsedTime % 60).ToString("f1");
        */
    }

    
}
