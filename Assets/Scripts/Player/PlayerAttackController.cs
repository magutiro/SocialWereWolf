using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using Unity.Netcode;

public class PlayerAttackController : NetworkBehaviour
{
    public UIController _uIController;
    public float attackDistance = 5;


    public ReactiveProperty<int> Attack = new ReactiveProperty<int>(3);
    Button AttackButton;

    void SetInit()
    {
        _uIController = GameObject.Find("UIController").GetComponent<UIController>();

        if (!IsOwner) return;
        AttackButton = GameObject.Find("AttackButton").GetComponent<Button>();
        AttackButton.onClick.AddListener(() => OnAttackButton());
    }
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += Sceneloaded;
    }

    // Update is called once per frame
    void Update()
    {
        if(!_uIController && SceneManager.GetActiveScene().name == "InGameScene")
        {
            SetInit();
        }
        if (IsOwner && _uIController && _uIController._targetPlayer)
        {
            float distance = (_uIController._targetPlayer.transform.position - transform.position).sqrMagnitude;
            if (distance > attackDistance)
            {
                _uIController._targetPlayer = null;
            }
        }
    }
    public void OnAttackButton()
    {
        if (IsOwner && _uIController && _uIController._targetPlayer)
        {
            var pl = _uIController._targetPlayer.GetComponent<PlayerHPController>();
            Debug.Log(pl.playerHp.Value - Attack.Value);
            pl.SetHp(-Attack.Value);

        }
    }
    void Sceneloaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "InGameScene")
        {
            SetInit();
        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!_uIController || !IsOwner) return;
        if (collision.gameObject.tag == "OtherPlayer")
        {
            _uIController._targetPlayer = collision.gameObject.GetComponent<PlayerController>();
        }

    }
}
