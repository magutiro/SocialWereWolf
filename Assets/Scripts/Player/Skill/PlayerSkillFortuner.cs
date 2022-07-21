using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillFortuner : MonoBehaviour,SkillBase
{
    List<PlayerController> players = new List<PlayerController>();
    Dictionary<PlayerController, float> CountDic = new Dictionary<PlayerController, float>();

    [SerializeField]
    float time = 120f;

    [SerializeField]
    float distance = 5f;

    public void Effect1()
    {
        for (int i = 0; i < players.Count; i++)
        {
            //players[i]._hp += 3;
        }
    }
    public void Effect2()
    {
        //gameObject.GetComponent<PlayerController>()._hp += 5;
    }
    public void Update()
    {
        foreach(var p in CountDic)
        {
            if(p.Value > time)
            {
                Debug.Log(p.Key.GetComponent<PlayerJobState>().playerjob + "Ç™ñêEÇæ");
            }
        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "OtherPlayer")
        {
            var p = collision.gameObject.GetComponent<PlayerController>();
            if (CountDic.ContainsKey(p))
            {
                CountDic[p]+= Time.deltaTime;
            }
            else
            {
                CountDic.Add(collision.gameObject.GetComponent<PlayerController>(), 0f);
            }
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "OtherPlayer")
        {
            players.Remove(collision.gameObject.GetComponent<PlayerController>());
        }

    }
}
