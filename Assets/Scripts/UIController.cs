using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;

public class UIController : MonoBehaviour
{
    public PlayerController _targetPlayer;

    public GameObject hitterObject;

    public enum USEState
    {
        Work,
        Item,
        Dor
    }
    public USEState useState;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name =="LobbyScene" && NetworkManager.Singleton.IsServer && NetworkManager.Singleton.ConnectedClientsList.Count == 3)
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
    public void OnUseButton()
    {
        switch (useState)
        {
            case USEState.Dor:
                break;
            case USEState.Item:
                break;
            case USEState.Work:
                hitterObject.GetComponent<WorkController>().OnUseButton();
                break;
        }
    }
    public void OnSkillButton()
    {

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
