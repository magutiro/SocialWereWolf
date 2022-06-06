using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class Player
{
    public float _playerSpeed;
    public int _playerAttack;
    public int _playerHP;

    public Player(float speed, int attck, int hp)
    {
        this._playerSpeed = speed;
        this._playerAttack = attck;
        this._playerHP = hp;
    }
}
public class PlayerController : NetworkBehaviour
{
    bool ishit = false;
    [SerializeField]
    private Vector3 _moveVector;

    Rigidbody2D rgd2D;
    SpriteRenderer _sprite;
    InputAction move;
    FixedJoystick joystick;

    Player _player;
    GameObject parent;

    public  VivoxManager _vivoxManager;
    // Start is called before the first frame update
    void Start()
    {
        _player = new Player(10f, 2, 20);
        SceneManager.sceneLoaded += SceneUnloaded;
        Initialization();
    }
    private void Initialization()
    {
        //新しいInputシステム
        var playerInput = GetComponent<PlayerInput>();
        move = playerInput.actions["Move"]; // "Move" Actionを利用する。

        rgd2D = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();

        parent = GameObject.Find("PlayerManager");
        joystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();
        _vivoxManager = parent.GetComponent<VivoxManager>();

        var otherNameText = transform.Find("Name").gameObject;
        otherNameText.GetComponent<TextMesh>().text = UserLoginData.userName;

        if (!IsOwner)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        Debug.Log("初期化");
    }
    void SceneUnloaded(Scene scene, LoadSceneMode mode)
    {
        Initialization();
    }
    void Awake()
    {
        if (!_vivoxManager)
        {
            parent = GameObject.Find("PlayerManager");
            _vivoxManager = parent.GetComponent<VivoxManager>();
        }
        if (IsOwner && _vivoxManager)
        {
            _vivoxManager.JoinChannel("test1", VivoxUnity.ChannelType.Positional);
            Debug.Log("joinVC");
        }
    }

    // Update is called once per frame
    void Update()
    {
        rgd2D.velocity = Vector3.zero;
        if (!ishit && IsOwner)
        {
            // 横矢印キーの押されている状況を取得
            var inputMoveAxis = move.ReadValue<Vector2>();
            //キーボードの入力か仮想パットの入力を取得
            inputMoveAxis.x = inputMoveAxis.x == 0 ? joystick.Horizontal : inputMoveAxis.x;
            inputMoveAxis.y = inputMoveAxis.y == 0 ? joystick.Vertical : inputMoveAxis.y;
            //入力がない場合に動きを止める
            if (inputMoveAxis.y == 0 && inputMoveAxis.x == 0)
            {
                rgd2D.velocity = Vector3.zero;
                _moveVector = Vector3.zero;
                StopMoveServerRpc();
            }
            else
            {
                //サーバー側に入力Vectorを送信
                SetMoveInputServerRPc(inputMoveAxis);
            }
        }
        if (IsServer)
        {
            // サーバー側は移動処理を実行
            MovePlayer();
        }
    }
    /// <summary>
    /// [ServerRpc]を使うことで、サーバー側で実行されるメソッドになる
    /// </summary>
    [ServerRpc]
    private void SetMoveInputServerRPc(Vector2 Axis)
    {
        _moveVector = Axis;
        FlipChange(_moveVector.x); 
    }
    [ServerRpc]
    void StopMoveServerRpc()
    {
        _moveVector = Vector3.zero;
        rgd2D.velocity = Vector3.zero;

    }
    public void MovePlayer()
    {
        _moveVector = _moveVector.normalized;
        rgd2D.velocity = _moveVector * _player._playerSpeed;
        FlipChange(_moveVector.x);
    }
    public void FlipChange(float x)
    {
        if(x == 0)
        {
            _sprite.flipX = _sprite.flipX;
        }
        else
        {
            _sprite.flipX = x > 0 ? true : false;
        }
    }

    public bool Killed(string name)
    {
        return false;
    }

}
