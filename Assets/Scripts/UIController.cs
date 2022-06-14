using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;

public class UIController : MonoBehaviour
{
    public PlayerController _targetPlayer;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(NetworkManager.Singleton.IsServer && NetworkManager.Singleton.ConnectedClientsList.Count == 9)
        {
            ServerScene();
        }
    }
    public void OnKillButton()
    {
        if (_targetPlayer)
        {
            Debug.Log(_targetPlayer.Killed(UserLoginData.userName.Value));
        }
    }
    public void OnStartButton()
    {
        SceneServerRpc();
    }

    [ServerRpc]
    void SceneServerRpc()
    {
        ServerScene();
    }
    private void ServerScene()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("InGameScene", LoadSceneMode.Single);

        }
    }


}
