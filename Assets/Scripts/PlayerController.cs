using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Photon.Pun;

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
public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    bool ishit = false;
    Vector3 moveVector;

    Rigidbody2D rgd2D;
    SpriteRenderer _sprite;
    InputAction move;
    FixedJoystick joystick;

    Player _player;
    GameObject parent;

    VivoxManager _vivoxManager;
    // Start is called before the first frame update
    void Start()
    {
        var playerInput = GetComponent<PlayerInput>();
        move = playerInput.actions["Move"]; // ← "Move" Actionを利用する。
        rgd2D = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _player = new Player(10f,2,20);

        parent = GameObject.Find("PlayerManager");
        joystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();
        gameObject.transform.parent = parent.gameObject.transform;
        _vivoxManager = parent.GetComponent<VivoxManager>();

        var otherNameText = transform.Find("Name").gameObject;
        otherNameText.GetComponent<TextMesh>().text = $"{photonView.Owner.NickName}({photonView.OwnerActorNr})";

        if (!photonView.IsMine)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

    }
    void Awake()
    {
        if (_vivoxManager)
        {
            _vivoxManager.JoinChannel("test1", VivoxUnity.ChannelType.Positional);
            Debug.Log("joinVC");
        }
    }

    // Update is called once per frame
    void Update()
    {
        rgd2D.velocity = Vector3.zero;
        moveVector = Vector3.zero;
        if (!ishit && gameObject.tag=="Player" && photonView.IsMine)
        {
            // 横矢印キーの押されている状況を取得
            var inputMoveAxis = move.ReadValue<Vector2>();
            inputMoveAxis.x = inputMoveAxis.x == 0 ? joystick.Horizontal : inputMoveAxis.x;
            inputMoveAxis.y = inputMoveAxis.y == 0 ? joystick.Vertical : inputMoveAxis.y;
            if (inputMoveAxis.y == 0 && inputMoveAxis.x == 0)
            {
                rgd2D.velocity = Vector3.zero;
                moveVector = Vector3.zero;
            }
            else
            {
                //print("Horizontal: " + joystick.Horizontal);
                //print("Vertical: " + joystick.Vertical);

                moveVector.x = inputMoveAxis.x;
                moveVector.y = inputMoveAxis.y;
                MovePlayer(moveVector);
                //transform.Translate(moveVector / 40);
            }
        }
        if (!photonView.IsMine)
        {
            FlipChange(rgd2D.velocity.x);
        }
    }

    public void MovePlayer(Vector3 pos)
    {
        pos = pos.normalized;
        rgd2D.velocity = pos * _player._playerSpeed;
        FlipChange(pos.x);
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
        photonView.RPC(nameof(RpcSendMessage), RpcTarget.AllViaServer, photonView.Owner.NickName+"が" +name+"にkillされました", photonView.Owner.NickName);
        return false;
    }
    [PunRPC]
    private void RpcSendMessage(string message, string Tname, PhotonMessageInfo info)
    {
        if(Tname == UserLoginData.userName)
        {
            _sprite.color = Color.red;
        }
        Debug.Log(message);
        Debug.Log(Tname + UserLoginData.userName);
    }
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 自身のアバターの色を送信する
            stream.SendNext(_sprite.color.r);
            stream.SendNext(_sprite.color.g);
            stream.SendNext(_sprite.color.b);
            stream.SendNext(_sprite.color.a);
            stream.SendNext(_sprite.flipX);
        }
        else
        {
            // 他プレイヤーのアバターの色を受信する
            float r = (float)stream.ReceiveNext();
            float g = (float)stream.ReceiveNext();
            float b = (float)stream.ReceiveNext();
            float a = (float)stream.ReceiveNext();
            _sprite.color = new Vector4(r, g, b, a);
            _sprite.flipX = (bool)stream.ReceiveNext();
        }
    }

}
