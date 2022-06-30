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

    public List<Button> _playerVoteButtons;
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
        _voteController = GameObject.Find("VoteController").GetComponent<VoteController>();
        for(int i = 0; i < 9; i++)
        {
            _playerVoteButtons[i] = GameObject.Find("PlayerVoteButton�i"+i+"�j").GetComponent<Button>();
            _playerVoteButtons[i].onClick.AddListener(() => OnSetVoteButton(i));
        }
        GameObject.Find("VoteButton").GetComponent<Button>().onClick.AddListener(() => OnVoteButton());
    }
    //���[�{�^�����������Ƃ��̏���
    public void OnVoteButton()
    {
        _voteController.SetVoteServerRpc(_targetPlayerId);
    }
    //���[��̃v���C���[���������Ƃ��̏���
    public void OnSetVoteButton(int playerId)
    {
        _targetPlayerId = playerId;
    }

    public void ResetVoteImages()
    {
        GameObject parent = _playerVoteButtons[0].transform.parent.gameObject;
        for(int p = 0; p < 9; p++)
        {
            ColorBlock colorblock = _playerVoteButtons[p].colors;
            if (_playerManager.playerList[p].GetComponent<PlayerController>()._player.playerState == Player.PlayerState.Alive)
            {

            }
            else
            {
                colorblock.normalColor = new Color32((byte)colorblock.normalColor.r, (byte)colorblock.normalColor.g, (byte)colorblock.normalColor.b, 0);
                _playerVoteButtons[p].transform.parent = null;
                _playerVoteButtons[p].transform.parent = parent.transform;
                
            }
            _playerVoteButtons[p].colors = colorblock;

        }
    }
}
