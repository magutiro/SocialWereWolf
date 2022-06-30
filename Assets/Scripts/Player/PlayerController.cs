using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UniRx;

public class Player
{
    public float _playerSpeed;
    public int _playerAttack;
    public int _playerHP;
    public int _jobSkillID;
    public int _baseSkillID;
    public PlayerState playerState = PlayerState.Alive;
    public enum PlayerState
    {
        Alive,
        Dead
    }

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
    bool isOp = true;
    [SerializeField]
    private Vector3 _moveVector;

    Rigidbody2D rgd2D;
    SpriteRenderer _sprite;
    InputAction move;
    FixedJoystick joystick;

    public Player _player;
    GameObject parent;

    public VivoxManager _vivoxManager;
    public GameController gameController;

    private NetworkVariable<int> _playerId =
        new NetworkVariable<int>();

    [SerializeField]
    private NetworkVariable<Vector2> _moveVector2 = new NetworkVariable<Vector2>();


    [SerializeField]
    private Vector2 _moveInput;

    [SerializeField]
    private NetworkVariable<NetworkString> _name = new NetworkVariable<NetworkString>();
    // Start is called before the first frame update
    void Start()
    {
        _player = new Player(10f, 2, 20);
        SceneManager.sceneLoaded += SceneUnloaded;
        if (IsOwner)
        {
            setNameServerRpc(UserLoginData.userName.Value);
            Debug.Log(UserLoginData.userName.Value);
        }
        Initialization();
        
        transform.Find("PlayerManager").GetComponent<PlayerManager>().playerList.Add(this.gameObject);
    }
    [ServerRpc(RequireOwnership = true)]
    void setNameServerRpc(string name)
    {
        _name.Value = name;
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
        otherNameText.GetComponent<TextMesh>().text = _name.Value.ToString();

        if (!IsOwner)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        Debug.Log("初期化");
    }
    void SceneUnloaded(Scene scene, LoadSceneMode mode)
    {
        Initialization();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController._gameState
            .DistinctUntilChanged()
            .Subscribe(_ => StateInit());
        gameController.AddObservable
            .Where(x => x.Value == UserLoginData.userName.Value)
            .Subscribe(x => _playerId.Value = x.Key);
        PlayerSpwnPoint playerSpwnPoint = GameObject.Find("PlayerSpwnPoint").GetComponent<PlayerSpwnPoint>();
        playerSpwnPoint.SpawnPlayer(this.gameObject, _playerId.Value);
    }
    private void StateInit()
    {
        switch (gameController._gameState.Value)
        {
            case GameState.Morning:
                isOp = false;
                break;
            case GameState.Daytime:
                isOp = true;
                break;
            case GameState.Evening:
                isOp = false;
                break;
            case GameState.Night:
                isOp = false;
                break;
        }
    }
    void Awake()
    {
#if CLIENT
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
#endif
    }

    // Update is called once per frame
    void Update()
    {
        //rgd2D.velocity = Vector3.zero;
        //_moveVector = Vector3.zero;
        //_moveVector2.Value = Vector2.zero;
        if (!ishit && IsOwner && isOp)
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
                SetMoveInputServerRpc(inputMoveAxis);
                //_moveVector = Vector3.zero;
                //_moveVector2.Value = Vector2.zero;
            }
            else
            {
                //サーバー側に入力Vectorを送信
                _moveVector = inputMoveAxis;
                SetMoveInputServerRpc(inputMoveAxis);
                //MovePlayerServerRpc();
            }
        }
        if (IsServer)
        {
            if (_moveVector2.Value.y == 0 && _moveVector2.Value.x == 0)
            {
                rgd2D.velocity = Vector3.zero;
            }
            else
            {
                MovePlayer();
            }
        }
    }
    /// <summary>
    /// [ServerRpc]を使うことで、サーバー側で実行されるメソッドになる
    /// メソッド名の語尾にServerRpcがない場合エラーとなる
    /// </summary>
    [ServerRpc]
    private void SetMoveInputServerRpc(Vector2 Axis)
    {
        _moveVector = Axis;
        _moveVector2.Value = Axis;
        FlipChangeClientRpc(_moveVector.x);
    }
    [ServerRpc]
    void MovePlayerServerRpc()
    {
        _moveVector = _moveVector.normalized;
        _moveVector2.Value = _moveVector2.Value.normalized;
        rgd2D.velocity = _moveVector2.Value * _player._playerSpeed;
        FlipChangeClientRpc(_moveVector2.Value.x);
    }
    public void MovePlayer()
    {

        _moveVector = _moveVector.normalized;
        _moveVector2.Value = _moveVector2.Value.normalized;
        rgd2D.velocity = _moveVector2.Value * _player._playerSpeed;
        FlipChangeClientRpc(_moveVector2.Value.x);
    }
    [ClientRpc]
    public void FlipChangeClientRpc(float x)
    {
        if (_sprite == null) return;
        if (x == 0)
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
        KillClientRpc(name);
        return false;
    }
    [ClientRpc]
    void KillClientRpc(string name)
    {
        _sprite.color = Color.red;
        _player.playerState = Player.PlayerState.Dead;
        Debug.Log(UserLoginData.userName + "が" + name + "に殺されました");
    }

}
