using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Unity.Netcode;
using System;

public class PlayerManager : NetworkBehaviour { 

    public GameController gameController;
    public List<GameObject> playerList;
    public GameObject myPlayer;

    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += Sceneloaded;
        SceneManager.activeSceneChanged += SceneChanged;
        SceneManager.sceneUnloaded += SceneUnloaded;
        Debug.Log("PlayerManagerStart");
    }


    void Awake()
    {

        Debug.Log("PlayerManagerAwake");
    }
    void Update()
    {

    }
    private void SceneChanged(Scene arg0, Scene arg1)
    {

    }
    private void Sceneloaded(Scene scene, LoadSceneMode arg1)
    {
        if(scene.name == "InGameScene")
        {
            gameController = GameObject.Find("GameController").GetComponent<GameController>();
            gameController.pm = this;
            Debug.Log("PlayerManagerSceneLoad");
        }
        if (scene.name == "LobbyScene")
        {
            //DontDestroyOnLoad(gameObject);
        }
    }
    void SceneUnloaded(Scene scene)
    {
        
        if(scene.name == "InGameScene")
        {
            SceneManager.MoveGameObjectToScene(gameObject, scene);
            Debug.Log("PlayerManagerSceneUnLoad");
        }
    }

    /*
    [ServerRpc]
    public void SpawnObjectServerRpc(GameObject gameObject)
    {
        Add(gameObject);
    }
    public void Add(GameObject playerController)
    {
        playerList.Add(playerController);
    }
    */
}
