using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMeetFlame
{
    public Button _button;
    public Text _text;
    public Sprite _sprite;

    public void OnVoteButton()
    {

    }
}
public class MeetController : MonoBehaviour
{
    public List<PlayerMeetFlame> meet;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 9; i++)
        {
            meet.Add(GameObject.Find("VoteButton ("+i+")").GetComponent<PlayerMeetFlame>());
        }

        foreach(var playerMeetFlame in meet)
        {
            playerMeetFlame._button.onClick.AddListener(playerMeetFlame.OnVoteButton);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnVote()
    {

    }
}
