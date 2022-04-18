using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class WolfController : MonoBehaviourPunCallbacks
{
    public UIController _uIController;
    public float killDistance = 5;
    // Start is called before the first frame update
    void Start()
    {
        _uIController = GameObject.Find("UIController").GetComponent<UIController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_uIController._targetPlayer)
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
        if (collision.gameObject.tag == "OtherPlayer")
        {
            _uIController._targetPlayer = collision.gameObject.GetComponent<PlayerController>();
        }

    }
}
