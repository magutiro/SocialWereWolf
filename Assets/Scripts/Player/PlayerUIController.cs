using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    public UIController _uIController;
    // Start is called before the first frame update
    void Start()
    {
        _uIController = GameObject.Find("UIController").GetComponent<UIController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_uIController)
        {
            _uIController = GameObject.Find("UIController").GetComponent<UIController>();
        }
    }


    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "OtherPlayer")
        {
            _uIController._targetPlayer = collision.gameObject.GetComponent<PlayerController>();
        }
        if (collision.gameObject.tag == "Work")
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
