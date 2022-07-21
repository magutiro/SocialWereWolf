using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillHealer : MonoBehaviour, SkillBase
{
    List<PlayerHPController> players = new List<PlayerHPController>();

    public void Effect1()
    {
        for(int i = 0; i < players.Count; i++)
        {
            players[i].HP.Value += 3;
        }
    }
    public void Effect2()
    {
        gameObject.GetComponent<PlayerHPController>().HP.Value += 5;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "OtherPlayer")
        {
            players.Add(collision.gameObject.GetComponent<PlayerHPController>());
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "OtherPlayer")
        {
            players.Remove(collision.gameObject.GetComponent<PlayerHPController>());
        }

    }
}
