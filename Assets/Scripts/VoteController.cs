using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class VoteController : NetworkBehaviour
{
    public Dictionary<int, int> _playerVoteDic;
    public PlayerManager playerManager;
    public List<Button> _playerVoteButtons = new List<Button>();
    public List<PlayerVoteController> _playerVoteControllers = new List<PlayerVoteController>();


    NetworkVariable<int> voteCount = new NetworkVariable<int>(0);

    private void Start()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        var mp= playerManager.myPlayer.GetComponent<PlayerVoteController>();
        for (int i = 0; i < playerManager.playerList.Count; i++)
        {
            _playerVoteControllers.Add(playerManager.playerList[i].GetComponent<PlayerVoteController>());
            _playerVoteButtons[i].onClick.AddListener(() => mp.OnSetVoteButton(i));
        }
    }

    private void Update()
    {
        if(voteCount.Value == 9)
        {
            VoteSubmit();
        }
    }
    public void VoteSubmit()
    {
        int pid = 0;
        bool isVote = true;
        foreach(var pv in _playerVoteDic)
        {
            if(pid < pv.Value)
            {
                pid = pv.Value;
                isVote = true;
            }
            else if(pid == pv.Value)
            {
                isVote = false;
            }
        }
        if (isVote)
        {
            Debug.Log(pid + "ÇÃÉvÉåÉCÉÑÅ[Ç™èàåYÇ≥ÇÍÇ‹ÇµÇΩ");
        }
    }
    public void ResetVoteImages()
    {
        GameObject parent = _playerVoteButtons[0].transform.parent.gameObject;
        /*
        GameObject grandParent = parent.transform.parent.gameObject;

        for (int p = 0; p < 9; p++)
        {
            ColorBlock colorblock = _playerVoteButtons[p].colors;
            if (playerManager.playerList[p].GetComponent<PlayerController>()._player.playerState == Player.PlayerState.Alive)
            {
                Debug.Log(p + "ê∂ë∂");
            }
            else
            {
                colorblock.normalColor = new Color32((byte)colorblock.normalColor.r, (byte)colorblock.normalColor.g, (byte)colorblock.normalColor.b, 0);
                _playerVoteButtons[p].transform.parent.parent = GameObject.Find("MeetingPanel").transform;
                _playerVoteButtons[p].transform.parent.parent = grandParent.transform;

            }
            _playerVoteButtons[p].colors = colorblock;
        }
            */
    }
    [ServerRpc]
    public void SetVoteServerRpc(int PlayerID)
    {
        _playerVoteDic[PlayerID]++;
        voteCount.Value++;
    }
}
