using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
public class PlayerController : MonoBehaviour
{
    bool ishit = false;
    Vector3 moveVector;

    Rigidbody2D rgd2D;
    SpriteRenderer _sprite;
    InputAction move;

    Player _player;
    // Start is called before the first frame update
    void Start()
    {
        var playerInput = GetComponent<PlayerInput>();
        move = playerInput.actions["Move"]; // ← "Move" Actionを利用する。
        rgd2D = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _player = new Player(10f,2,20);
    }

    // Update is called once per frame
    void Update()
    {
        if (!ishit)
        {
            rgd2D.velocity = Vector3.zero;
            moveVector = Vector3.zero;
            // 横矢印キーの押されている状況を取得
            var inputMoveAxis = move.ReadValue<Vector2>();
            /*
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            */

            if (inputMoveAxis.x!= 0)
            {
                _sprite.flipX = inputMoveAxis.x > 0 ? true : false;
                if(inputMoveAxis.x > 0)
                {
                    WebSocketClientManager.SendPlayerAction("move", transform.position, "right", inputMoveAxis.x);
                }
                else
                {
                    WebSocketClientManager.SendPlayerAction("move", transform.position, "left", -inputMoveAxis.x);
                }

            }
            if(inputMoveAxis.y != 0)
            {
                if (inputMoveAxis.y > 0)
                {
                    WebSocketClientManager.SendPlayerAction("move", transform.position, "up", inputMoveAxis.y);
                }
                else
                {
                    WebSocketClientManager.SendPlayerAction("move", transform.position, "down", -inputMoveAxis.y);
                }
            }

            if (inputMoveAxis.y == 0 && inputMoveAxis.x == 0)
            {
                rgd2D.velocity = Vector3.zero;
                moveVector = Vector3.zero;
            }
            else
            {

                moveVector.x = inputMoveAxis.x;
                moveVector.y = inputMoveAxis.y;
                moveVector = moveVector.normalized;
                rgd2D.velocity = moveVector * _player._playerSpeed;

            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

    }
    private void OnCollisionExit2D(Collision2D collision)
    {

    }
}
