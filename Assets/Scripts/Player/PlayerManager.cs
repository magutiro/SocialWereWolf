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

    void SetInit()
    {

        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.pm = this;
    }
    void Awake()
    {

        Debug.Log("PlayerManagerAwake");
    }
    void Update()
    {
        if (!gameController && SceneManager.GetActiveScene().name == "InGameScene")
        {
            SetInit();
        }
    }
    private void SceneChanged(Scene arg0, Scene arg1)
    {

    }
    private void Sceneloaded(Scene scene, LoadSceneMode arg1)
    {
        
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
