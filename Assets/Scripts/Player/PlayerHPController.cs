using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class PlayerHPController : MonoBehaviour
{
    [SerializeField]
    List<GameObject> _hpSell;

    PlayerController player;

    int hp;
    ReactiveProperty<int> HP = new ReactiveProperty<int>(1);


    // Start is called before the first frame update
    void Start()
    {
        HP.Value = player._player._playerHP;

        HP.Zip(HP.Skip(1), (x, y) => new {
            OldValue = x,
            NewValue = y
        }).Subscribe(t => 
            HPChanged(t.OldValue < t.NewValue)
            ) ;
    }
    void HPChanged(bool isHeal)
    {
        //HP‚ª‘‚¦‚½ê‡—Î‚ð•\Ž¦‚·‚é
        if (isHeal)
        {
            _hpSell[HP.Value].transform.GetChild(1).gameObject.SetActive(false);
            _hpSell[HP.Value].transform.GetChild(0).gameObject.SetActive(true);
        }
        //HP‚ªŒ¸‚Á‚½ê‡Ô‚ð•\Ž¦‚·‚é
        else
        {
            _hpSell[HP.Value].transform.GetChild(0).gameObject.SetActive(false);
            _hpSell[HP.Value].transform.GetChild(1).gameObject.SetActive(true);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
