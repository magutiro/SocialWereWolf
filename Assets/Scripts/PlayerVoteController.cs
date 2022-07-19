using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System;
using System.Linq;

public class PlayerVoteController :MonoBehaviour
{
    PlayerController _playerController;
    Player _player;
    int _targetPlayerId;
    PlayerManager _playerManager;
    VoteController _voteController;

    public List<Button> _playerVoteButtons = new List<Button>();
    //初期化
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
        _voteController = GameObject.Find("VoteController").GetComponent<VoteController>();
        
        //GameObject.Find("VoteButton").GetComponent<Button>().onClick.AddListener(() => OnVoteButton());
    }
    //投票ボタンを押したときの処理
    public void OnVoteButton()
    {
        _voteController.SetVoteServerRpc(_targetPlayerId);
    }
    //投票先のプレイヤーを押したときの処理
    public void OnSetVoteButton(int playerId)
    {
        _targetPlayerId = playerId;
    }

}
