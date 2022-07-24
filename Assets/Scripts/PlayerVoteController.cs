using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System;
using System.Linq;
using TMPro;
public class PlayerVoteController :NetworkBehaviour
{
    PlayerController _playerController;
    Player _player;
    [SerializeField]
    int _targetPlayerId = 0;
    PlayerManager _playerManager;

    [SerializeField]
    VoteController _voteController;

    //������
    private void Init()
    {
        _playerController = GetComponent<PlayerController>();
        _player = _playerController._player;
        _playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
    }
    private void Start()
    {
        Init(); 
        SceneManager.sceneLoaded += SceneUnloaded;
    }
    void SceneUnloaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "InGameScene") return;
        _voteController = GameObject.Find("VoteController").GetComponent<VoteController>();
        
        //GameObject.Find("VoteButton").GetComponent<Button>().onClick.AddListener(() => OnVoteButton());
    }
    //���[�{�^�����������Ƃ��̏���
    public void OnVoteButton()
    {
        _voteController = GameObject.Find("VoteController").GetComponent<VoteController>();
        _voteController.SetVoteServerRpc(_targetPlayerId);
        Debug.Log(_targetPlayerId+"�ɓ��[���܂����B");
    }
    //���[��̃v���C���[���������Ƃ��̏���
    public void OnSetVoteButton(int playerId)
    {
        Debug.Log(playerId);
        _targetPlayerId = playerId;
        _voteController.voteText.text = _playerManager.playerList[_targetPlayerId].GetComponent<PlayerController>()._name.Value + "�ɓ��[��";
    }

}
