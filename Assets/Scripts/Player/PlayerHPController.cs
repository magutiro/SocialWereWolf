using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class PlayerHPController : NetworkBehaviour
{
    [SerializeField]
    List<GameObject> _hpSell;

    PlayerController player;

    //public ReactiveProperty<int> HP = new ReactiveProperty<int>(10);
    public NetworkVariable<int> playerHp = new NetworkVariable<int>(10);


    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerController>();

        SceneManager.sceneLoaded += Sceneloaded;

        /*
        HP.Zip(HP.Skip(1), (x, y) => new
        {
            OldValue = x,
            NewValue = y
        }).Subscribe(t =>
            HPChangedClientRpc(t.OldValue < t.NewValue, t.NewValue)
        );
        */
        if (IsServer)
        {
            playerHp.Value = player._player._playerHP;
        }
        playerHp.OnValueChanged += OnChangedPlayerHP;
    }
    
    void Sceneloaded(Scene scene, LoadSceneMode mode)
    {
        if (!IsOwner || scene.name != "InGameScene") return;
        var HPbar = GameObject.Find("HPBAR");
        _hpSell = new List<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            _hpSell.Add(HPbar.transform.GetChild(i).gameObject);
        }
    }
    void OnChangedPlayerHP(int prev, int current)
    {
        HPUIChanged(prev < current, current);
    }
    public void SetHp(int hp)
    {
        SetHPServerRpc(playerHp.Value + hp);
    }
    [ServerRpc(RequireOwnership = false)]
    void SetHPServerRpc(int hp)
    {
        playerHp.Value = hp;
    }
    void HPUIChanged(bool isHeal, int hp)
    {
#if CLIENT
        if (!IsOwner) return;
        Debug.Log(player._name.Value +"\n" +UserLoginData.userName.Value);
        if (hp > 10)
        {
            SetHPServerRpc(10);
            return;
        }
        int hpCount = _hpSell.Count;
        if (hp <= 0) 
        { 
            Dead();
            for (int i = 0; i < hpCount; i++)
            {
                _hpSell[i].transform.GetChild(0).gameObject.SetActive(false);
                _hpSell[i].transform.GetChild(1).gameObject.SetActive(true);
            }
        }
        else
        {
            if (isHeal)
            {
                //HP‚ª‘‚¦‚½ê‡—Î‚ð•\Ž¦‚·‚é
                for (int i = 0; i <hpCount; i++)
                {
                    _hpSell[i].transform.GetChild(1).gameObject.SetActive(false);
                    _hpSell[i].transform.GetChild(0).gameObject.SetActive(true);

                }
            }
            else
            {
                //HP‚ªŒ¸‚Á‚½ê‡Ô‚ð•\Ž¦‚·‚é
                for (int i = hp - 1; i < _hpSell.Count; i++)
                {
                    _hpSell[i].transform.GetChild(0).gameObject.SetActive(false);
                    _hpSell[i].transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            
        }
#endif
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
