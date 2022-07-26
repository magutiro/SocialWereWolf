using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UniRx;
using TMPro;
using System;

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
    [SerializeField]
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

    public NetworkVariable<NetworkString> _name = new NetworkVariable<NetworkString>();

    TextMesh nameText = new TextMesh();

    private PlayerAnimController playerAnimController;

    void Start()
    {
        _player = new Player(10f, 2, 10);
        GameObject.Find("PlayerManager").GetComponent<PlayerManager>().playerList.Add(this.gameObject);

        SceneManager.sceneLoaded += Sceneloaded;
        SceneManager.sceneUnloaded += SceneUnloaded;
        Initialization();
    }

    private void SceneUnloaded(Scene scene)
    {
        if (scene.name == "InGameScene")
        {
            //Destroy(this.gameObject);
        }
    }

    [ServerRpc(RequireOwnership = true)]
    void setNameServerRpc(string name)
    {
        _name.Value = name;
    }
    private void Initialization()
    {
        //�V����Input�V�X�e��
        if (!gameObject) return;

        rgd2D = GetComponent<Rigidbody2D>();
#if CLIENT
        var playerInput = GetComponent<PlayerInput>();
        move = playerInput.actions["Move"]; // "Move" Action�𗘗p����B


        joystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();
        _vivoxManager = GameObject.Find("Vivox").GetComponent<VivoxManager>();
        nameText =transform.GetChild(1).GetComponent<TextMesh>();
        nameText.text = _name.Value.ToString();

        if (IsOwner)
        {
            setNameServerRpc(UserLoginData.userName.Value);
            GameObject.Find("PlayerManager").GetComponent<PlayerManager>().myPlayer = this.gameObject;
        }
#endif

        playerAnimController = GetComponent<PlayerAnimController>();
        if (!IsOwner)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    void InitInGame()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        Debug.Log("<color=red>GameController�̐ݒ�</color>" + gameController);
        Initialization();
        gameController._gameState
            .DistinctUntilChanged()
            .Subscribe(_ => StateInit());
        gameController.AddObservable
            .Where(x => x.Value == UserLoginData.userName.Value)
            .Subscribe(x => _playerId.Value = x.Key);
        if (IsServer)
        {
            PlayerSpwnPoint playerSpwnPoint = GameObject.Find("PlayerSpwnPoint").GetComponent<PlayerSpwnPoint>();
            playerSpwnPoint.SpawnPlayer(this.gameObject, _playerId.Value);

        }
    }
    void Sceneloaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "InGameScene")
        {
            InitInGame();
        }
        
    }
    private void StateInit()
    {
        if (!IsOwner) return;
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
            _vivoxManager = GameObject.Find("Vivox").GetComponent<VivoxManager>();
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
        if (!gameController)
        {
            if (SceneManager.GetActiveScene().name == "InGameScene")
            {
                Initialization();
                InitInGame();

            }
        }
        if(nameText.text != _name.Value)
        {
            nameText.text = _name.Value.ToString();
        }
        //rgd2D.velocity = Vector3.zero;
        //_moveVector = Vector3.zero;
        //_moveVector2.Value = Vector2.zero;
        if (!ishit && IsOwner && isOp)
        {
            // �����L�[�̉�����Ă���󋵂��擾
            var inputMoveAxis = move.ReadValue<Vector2>();
            //�L�[�{�[�h�̓��͂����z�p�b�g�̓��͂��擾
            inputMoveAxis.x = inputMoveAxis.x == 0 ? joystick.Horizontal : inputMoveAxis.x;
            inputMoveAxis.y = inputMoveAxis.y == 0 ? joystick.Vertical : inputMoveAxis.y;
            //���͂��Ȃ��ꍇ�ɓ������~�߂�
            if ( gameController!=null&&gameController._gameState.Value != GameState.Daytime && (inputMoveAxis.y == 0 && inputMoveAxis.x == 0))
            {
                rgd2D.velocity = Vector3.zero;
                SetMoveInputServerRpc(inputMoveAxis);
                //_moveVector = Vector3.zero;
                //_moveVector2.Value = Vector2.zero;
            }
            else
            {
                
                //�T�[�o�[���ɓ���Vector�𑗐M
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
                playerAnimController.StopAnimServerRpc();
            }
            else
            {
                MovePlayer();
                playerAnimController.StartAnimServerRpc();
            }
        }
        if (_moveVector2.Value.x > 0)
        {
            transform.GetChild(2).localScale = new Vector3(-1, 1, 1);
        }
        else if(_moveVector2.Value.x < 0)
        {
            transform.GetChild(2).localScale = new Vector3(1, 1, 1);
        }
    }
    /// <summary>
    /// [ServerRpc]���g�����ƂŁA�T�[�o�[���Ŏ��s����郁�\�b�h�ɂȂ�
    /// ���\�b�h���̌����ServerRpc���Ȃ��ꍇ�G���[�ƂȂ�
    /// </summary>
    [ServerRpc]
    private void SetMoveInputServerRpc(Vector2 Axis)
    {
        _moveVector = Axis;
        _moveVector2.Value = Axis;
    }
    public void MovePlayer()
    {
        _moveVector = _moveVector.normalized;
        _moveVector2.Value = _moveVector2.Value.normalized;
        rgd2D.velocity = _moveVector2.Value * _player._playerSpeed;
    }
    [ServerRpc(RequireOwnership = false)]
    public void PlayerKilledServerRpc(string name, string targetName)
    {
        _player.playerState = Player.PlayerState.Dead;
        gameController.IsGameEnd();
        KillClientRpc(name, targetName);
        _name.Value = _name.Value + "���S";
    }
    [ClientRpc]
    void KillClientRpc(string name, string targetName)
    {
        _player.playerState = Player.PlayerState.Dead;
        Debug.Log(targetName + "��" + name + "�ɎE����܂���");
        GetComponent<NetworkObject>().Despawn();
    }
    [ServerRpc(RequireOwnership = false)]
    public void PlayerVoteDeadServerRpc()
    {
        _player.playerState = Player.PlayerState.Dead;
        gameController.IsGameEnd();
        VoteDeadClientRpc();
    }
    [ClientRpc]
    void VoteDeadClientRpc()
    {
        _player.playerState = Player.PlayerState.Dead;
    }

}
