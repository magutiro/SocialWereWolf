using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
public class PlayerManager : MonoBehaviour { 
    public List<string> _playerNames;
    // Start is called before the first frame update
    void Start()
    {

    }
    private void Awake()
    {
    }
    void Update()
    {
        
    }
    private void OnApplicationQuit()
    {
    }
    private void EndWebsocket()
    {
    }
    public void OnClickExitButton()
    {
        // �^�C�g���V�[���ɖ߂�
        SceneManager.LoadScene("TitleScene");
    }

}
