using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class Server : MonoBehaviour
{
    private Vector2 _moveInput;
    public int a = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            return;
        }
        if (Keyboard.current.anyKey.isPressed)
        {
            SetMoveInputServerRpc();
        }
    }
    [Unity.Netcode.ServerRpc]
    private void SetMoveInputServerRpc()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("�T�[�o�[");
            a += 10;
        }
        // ��������l�́A�T�[�o�[���̃I�u�W�F�N�g�ɃZ�b�g�����
    }
}
