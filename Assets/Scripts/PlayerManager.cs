using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private GameObject playerPrefab = null;     // �v���C���[�̃��\�[�X(�v���n�u)
    private GameObject player;                  // ���v���C���[���
    private const float KEY_MOVEMENT = 0.5f;    // �ړ��{�^��1��N���b�N�ł̈ړ���

    // �S�v���C���[�̍s�����
    private Dictionary<string, PlayerActionData> PlayerActionMap;

    // �S�v���C���[�̃I�u�W�F�N�g���
    private readonly Dictionary<string, GameObject> playerObjectMap = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // ���v���C���[�̍쐬
        player = MakePlayer(new Vector3(-3, 3, 0), UserLoginData.userName);
        player.tag = "Player";
        // WebSocket�J�n
        StartWebSocket();
    }
    void Update()
    {
        // ���[�U�[�̍s����񂪂������瓯���������s���A���[�U�[�̍s������������
        if (PlayerActionMap != null)
        {
            Synchronaize();
            PlayerActionMap = null;
        }
    }
    /// <summary>
    /// WebSocket�̊J�n
    /// </summary>
    private void StartWebSocket()
    {
        // WebSocket�ʐM�J�n
        WebSocketClientManager.Connect();

        // WebSocket�̃��b�Z�[�W��M���\�b�h�̐ݒ�
        WebSocketClientManager.recieveCompletedHandler += OnReciveMessage;

        // ���v���C���[�̏�������WebSocket�ɑ��M
        WebSocketClientManager.SendPlayerAction("connect", new Vector3(-0, 0, 0), "neutral", 0.0f);
    }
    private void OnApplicationQuit()
    {
        EndWebsocket();
    }
    /// <summary>
    /// WebSocket�̏I��
    /// </summary>
    private void EndWebsocket()
    {
        WebSocketClientManager.SendPlayerAction("disconnect", Vector3.zero, "neutral", 0.0f);
        WebSocketClientManager.DisConnect();
    }
    private void OnReciveMessage(Dictionary<string, PlayerActionData> PlayerActionMap)
    {
        // ���������擾
        this.PlayerActionMap = PlayerActionMap;
    }

    /// <summary>
    /// ��������
    /// </summary>
    private void Synchronaize()
    {

        // �ޏo�������v���C���[�̌���
        List<string> otherPlayerNameList = new List<string>(playerObjectMap.Keys);
        foreach (var otherPlayerName in otherPlayerNameList)
        {
            // �ޏo�����v���C���[�̍폜
            if (!PlayerActionMap.ContainsKey(otherPlayerName))
            {
                Destroy(playerObjectMap[otherPlayerName]);
                playerObjectMap.Remove(otherPlayerName);
            }
        }

        // �v���C���[�̈ʒu���X�V
        foreach (var playerAction in PlayerActionMap.Values)
        {
            Debug.Log(playerAction.user+ GetMovePos(playerAction));
            // �����͈ړ��ς݂Ȃ̂ŃX���[
            if (UserLoginData.userName == playerAction.user)
            {
                continue;
            }

            // �������̑��v���C���[�̈ړ�
            if (playerObjectMap.ContainsKey(playerAction.user))
            {
                playerObjectMap[playerAction.user].transform.position = GetMovePos(playerAction);
                //playerObjectMap[playerAction.user].gameObject.GetComponent<PlayerController>().MovePlayer(GetMovePos(playerAction));
                // �������������v���C���[�̐���
            }
            else
            {
                // ���v���C���[�̍쐬
                Debug.Log("play2");
                var player = MakePlayer(GetMovePos(playerAction), playerAction.user);
                player.transform.GetChild(0).gameObject.SetActive(false);

                // ���v���C���[���X�g�ւ̒ǉ�
                playerObjectMap.Add(playerAction.user, player);
            }
        }
    }
    /// <summary>
    /// ��{�^���������̏���
    /// </summary>
    /// <summary>
    /// �ގ��{�^���������̏���
    /// </summary>
    public void OnClickExitButton()
    {
        // �^�C�g���V�[���ɖ߂�
        SceneManager.LoadScene("TitleScene");
    }

    /// <summary>
    /// �v���C���[���쐬
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="name"></param>
    private GameObject MakePlayer(Vector3 pos, string name)
    {
        // �v���C���[�̃��\�[�X(�v���n�u)���擾 ������̂�
        playerPrefab = playerPrefab ?? (GameObject)Resources.Load("Player");

        // �v���C���[�𐶐�
        var player = (GameObject)Instantiate(playerPrefab, pos, Quaternion.identity);

        // �v���C���[�̃l�[���v���[�g�̐ݒ�
        var otherNameText = player.transform.Find("Name").gameObject;
        otherNameText.GetComponent<TextMesh>().text = name;

        return player;
    }
    private Vector3 GetMovePos(PlayerActionData playerAction)
    {
        var pos = new Vector3(playerAction.pos_x, playerAction.pos_y, playerAction.pos_z);
        
        pos.y += (playerAction.way == "up") ? playerAction.range /40 : 0;
        pos.y -= (playerAction.way == "down") ? playerAction.range / 40 : 0;
        pos.x -= (playerAction.way == "left") ? playerAction.range / 40 : 0;
        pos.x += (playerAction.way == "right") ? playerAction.range / 40 : 0;
        return pos;
    }
}
