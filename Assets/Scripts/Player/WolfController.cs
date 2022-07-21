using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class WolfController : MonoBehaviour
{
    public UIController _uIController;
    public float killDistance = 5;
    void SceneUnloaded(Scene scene, LoadSceneMode mode)
    {
        _uIController = GameObject.Find("UIController").GetComponent<UIController>();
    }
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
            if (distance > killDistance)
            {
                _uIController._targetPlayer = null;
            }
        }
    }

    public void OnKillButton()
    {

    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!_uIController) return;
        if (collision.gameObject.tag == "OtherPlayer")
        {
            _uIController._targetPlayer = collision.gameObject.GetComponent<PlayerController>();
        }

    }
}
