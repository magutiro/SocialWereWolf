using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Unity.Netcode;
public class PlayerManager : MonoBehaviour { 

    public GameController gameController;
    public List<GameObject> playerList;
    // Start is called before the first frame update
    void Start()
    {

        SceneManager.sceneLoaded += SceneUnloaded;
    }
    void Awake()
    {

    }
    void Update()
    {

    }
    void SceneUnloaded(Scene scene, LoadSceneMode mode)
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();

    }
    [ServerRpc]
    public void SpawnObjectServerRpc(GameObject gameObject)
    {
        Add(gameObject);
    }
    public void Add(GameObject playerController)
    {
        playerList.Add(playerController);
    }
}
