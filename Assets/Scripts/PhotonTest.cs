using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public class PhotonTest : MonoBehaviourPunCallbacks
{
    //�@�V�[�����[�h���ɕ\������UI���
    [SerializeField]
    private GameObject loadUI = null;
    //�@�^�C�g���ɕ\������UI���
    [SerializeField]
    private GameObject inGameUI = null;

    private GameObject playerPrefab = null;
    // Start is called before the first frame update
    void Start()
    {
        // �v���C���[���g�̖��O��"Player"�ɐݒ肷��
        PhotonNetwork.NickName = UserLoginData.userName;
        // PhotonServerSettings�̐ݒ���e���g���ă}�X�^�[�T�[�o�[�֐ڑ�����
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
    // �}�X�^�[�T�[�o�[�ւ̐ڑ��������������ɌĂ΂��R�[���o�b�N
    public override void OnConnectedToMaster()
    {
        // "Room"�Ƃ������O�̃��[���ɎQ������i���[�������݂��Ȃ���΍쐬���ĎQ������j
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    // �Q�[���T�[�o�[�ւ̐ڑ��������������ɌĂ΂��R�[���o�b�N
    public override void OnJoinedRoom()
    {
        CreatePlayer();

        PhotonNetwork.AutomaticallySyncScene = true;
    }
    void CreatePlayer()
    {
        // �v���C���[�̃��\�[�X(�v���n�u)���擾 ������̂�
        playerPrefab = playerPrefab ?? (GameObject)Resources.Load("Player");
        // �����_���ȍ��W�Ɏ��g�̃A�o�^�[�i�l�b�g���[�N�I�u�W�F�N�g�j�𐶐�����
        var position = new Vector3(Random.Range(-2f, 2f), Random.Range(-3f, 3f));
        var player = PhotonNetwork.Instantiate("Player", position, Quaternion.identity);
        player.gameObject.tag = "Player";
        player.gameObject.transform.parent = gameObject.transform;
        //�@���[�h���UI���A�N�e�B�u�ɂ���
        loadUI.SetActive(false);
        //�@���[�h���UI���A�N�e�B�u�ɂ���
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
