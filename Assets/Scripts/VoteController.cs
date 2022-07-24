using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class VoteController : NetworkBehaviour
{
    public Dictionary<int, int> _playerVoteDic = new Dictionary<int, int>();
    public PlayerManager playerManager;
    public List<Button> _playerVoteButtons = new List<Button>();
    public GameController gm;
    public TextMeshProUGUI voteText = new TextMeshProUGUI();

    int _targetPlayerId = 0;
    NetworkVariable<int> voteCount = new NetworkVariable<int>(0);

    bool isVote = false;

    private void Start()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        gm = playerManager.gameController;

        GameObject parent = _playerVoteButtons[0].transform.parent.parent.gameObject;
        Debug.Log(parent.name);
        for (int i = 0; i < playerManager.playerList.Count; i++)
        {
            Debug.Log("ボタン"+i);
            var index = i;
            _playerVoteButtons[i].onClick.AddListener(() => OnSetVoteButton(index)); 
            TextMeshProUGUI tmpTEXT = parent.transform.GetChild(i).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
            tmpTEXT.text = playerManager.playerList[i].GetComponent<PlayerController>()._name.Value;
            _playerVoteDic.Add(i, 0);
        }
    }

    private void Update()
    {
        if(voteCount.Value == playerManager.playerList.Count)
        {
            if (IsServer)
            {
                voteCount.Value = 0;
                gm.SkipMeet();
            }
            else
            {
                VoteSubmit();
            }
        }
    }
    public void VoteSubmit()
    {
        int pid = 0;
        bool isVote = false;
        foreach(var pv in _playerVoteDic)
        {
            if(_playerVoteDic[pid] < pv.Value)
            {
                pid = pv.Key;
                isVote = true;
            }
            else if(_playerVoteDic[pid] > pv.Value)
            {

            }
            else
            {
                isVote = false;
            }
        }
        for(int i = 0; i < _playerVoteDic.Count; i++)
        {
            _playerVoteDic[i] = 0;
        }
        if (isVote)
        {
            Debug.Log(playerManager.playerList[pid].GetComponent<PlayerController>()._name.Value + "が処刑されました。");
            playerManager.playerList[pid].GetComponent<PlayerController>().PlayerVoteDeadServerRpc();
        }
        else
        {
            Debug.Log("同数により投票がスキップされました。");
        }
    }
    public void ResetVoteImages()
    {
        GameObject parent = _playerVoteButtons[0].transform.parent.gameObject;
        GameObject grandParent = parent.transform.parent.gameObject;

        for (int p = 0; p < playerManager.playerList.Count; p++)
        {
            ColorBlock colorblock = _playerVoteButtons[p].colors;
            if (playerManager.playerList[p].GetComponent<PlayerController>()._player.playerState == Player.PlayerState.Alive)
            {
                Debug.Log(p + "生存");
            }
            else
            {
                colorblock.normalColor = new Color32((byte)(colorblock.normalColor.r/2), (byte)(colorblock.normalColor.g/2), (byte)(colorblock.normalColor.b/2), 0);
                Debug.Log(colorblock.normalColor);
                _playerVoteButtons[p].transform.parent.parent = GameObject.Find("MeetingPanel").transform;
                _playerVoteButtons[p].transform.parent.parent = grandParent.transform;

            }
            _playerVoteButtons[p].colors = colorblock;
        }
        isVote = false;
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetVoteServerRpc(int PlayerID)
    {
        _playerVoteDic[PlayerID]++;
        voteCount.Value++;
        
    }
    //投票ボタンを押したときの処理
    public void OnVoteButton()
    {
        if (isVote)
        {
            return;
        }
        isVote = true;
        SetVoteServerRpc(_targetPlayerId);
        Debug.Log(_targetPlayerId + "に投票しました。");
    }
    //投票先のプレイヤーを押したときの処理
    public void OnSetVoteButton(int playerId)
    {
        Debug.Log(playerId + "に投票");
        _targetPlayerId = playerId;
        voteText.text = playerManager.playerList[_targetPlayerId].GetComponent<PlayerController>()._name.Value + "に投票中";
    }
}
