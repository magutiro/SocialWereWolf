using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Server : NetworkBehaviour
{
    private Vector2 _moveInput;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [Unity.Netcode.ServerRpc]
    private void SetMoveInputServerRPc(float x, float y)
    {
        // 代入した値は、サーバー側のオブジェクトにセットされる
        _moveInput = new Vector2(x, y);
    }
}
