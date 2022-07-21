using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;

public class PlayerHPController : MonoBehaviour
{
    [SerializeField]
    List<GameObject> _hpSell;

    PlayerController player;

    public ReactiveProperty<int> HP = new ReactiveProperty<int>(10);


    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerController>();

        SceneManager.sceneLoaded += SceneUnloaded;
        HP.Value = player._player._playerHP;

        HP.Zip(HP.Skip(1), (x, y) => new {
            OldValue = x,
            NewValue = y
        }).Subscribe(t => 
            HPChanged(t.OldValue < t.NewValue, t.NewValue)
            ) ;
    }
    void SceneUnloaded(Scene scene, LoadSceneMode mode)
    {
        var HPbar = GameObject.Find("HPBAR");
        _hpSell = new List<GameObject>();
        for (int i = 0; i < 9; i++)
        {
            _hpSell.Add(HPbar.transform.GetChild(i).gameObject);
        }
    }
        void HPChanged(bool isHeal, int hp)
    {
        if (hp <= 0) Dead();
        //HP‚ª‘‚¦‚½ê‡—Î‚ð•\Ž¦‚·‚é
        if (isHeal)
        {
            _hpSell[hp].transform.GetChild(1).gameObject.SetActive(false);
            _hpSell[hp].transform.GetChild(0).gameObject.SetActive(true);
        }
        //HP‚ªŒ¸‚Á‚½ê‡Ô‚ð•\Ž¦‚·‚é
        else
        {
            _hpSell[hp].transform.GetChild(0).gameObject.SetActive(false);
            _hpSell[hp].transform.GetChild(1).gameObject.SetActive(true);
        }
    }
    void Dead()
    {
        Debug.Log("Ž€–S‚µ‚Ü‚µ‚½");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
