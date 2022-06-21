using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class VoteController : NetworkBehaviour
{
    public Dictionary<int, int> _playerVoteDic;
    public PlayerManager playerManager;
    NetworkVariable<int> voteCount = new NetworkVariable<int>(0);

    private void Start()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
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
            Debug.Log(pid + "‚ÌƒvƒŒƒCƒ„[‚ªˆŒY‚³‚ê‚Ü‚µ‚½");
        }
    }
    [ServerRpc]
    public void SetVoteServerRpc(int PlayerID)
    {
        _playerVoteDic[PlayerID]++;
        voteCount.Value++;
    }
}
