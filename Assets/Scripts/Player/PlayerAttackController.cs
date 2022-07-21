using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;

public class PlayerAttackController : MonoBehaviour
{
    public UIController _uIController;
    public float attackDistance = 5;


    public ReactiveProperty<int> Attack = new ReactiveProperty<int>(3);
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += SceneUnloaded;
    }

    // Update is called once per frame
    void Update()
    {
        if (_uIController && _uIController._targetPlayer)
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
        _uIController._targetPlayer.GetComponent<PlayerHPController>().HP.Value -= Attack.Value;
    }
    void SceneUnloaded(Scene scene, LoadSceneMode mode)
    {
        _uIController = GameObject.Find("UIController").GetComponent<UIController>();
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!_uIController) return;
        if (collision.gameObject.tag == "OtherPlayer")
        {
            _uIController._targetPlayer = collision.gameObject.GetComponent<PlayerController>();
        }
        if (collision.gameObject.tag == "work")
        {
            _uIController.hitterObject = collision.gameObject;
            _uIController.useState = UIController.USEState.Work;
        }
        else if (collision.gameObject.tag == "item")
        {
            _uIController.hitterObject = collision.gameObject;
            _uIController.useState = UIController.USEState.Item;
        }
        else if (collision.gameObject.tag == "dor")
        {
            _uIController.hitterObject = collision.gameObject;
            _uIController.useState = UIController.USEState.Dor;
        }

    }
}
