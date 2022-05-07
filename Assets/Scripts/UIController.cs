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

    }
    public void OnKillButton()
    {
        if (_targetPlayer)
        {
            Debug.Log(_targetPlayer.Killed(UserLoginData.userName));
        }
    }
    public void OnStartButton()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("InGameScene", LoadSceneMode.Single);
    }

    //[ServerRpc]
    void Scene()
    {
        
    }


}
