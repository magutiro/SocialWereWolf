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

    public GameObject HPSells;
    // Start is called before the first frame update
    void Start()
    {

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
    }
    void Initialization()
    {
        player = GetComponent<PlayerController>();
        HPSells = GameObject.Find("HPBAR");
        _hpSell = new List<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            _hpSell.Add(HPSells.transform.GetChild(i).gameObject);
        }
        if (IsServer)
        {
            playerHp.Value = player._player._playerHP;
        }
        playerHp.OnValueChanged += OnChangedPlayerHP;
    }
    private void Update()
    {
        if (!HPSells && SceneManager.GetActiveScene().name == "InGameScene")
        {
            Initialization();
        }
    }

    void Sceneloaded(Scene scene, LoadSceneMode mode)
    {
        if (!IsOwner || scene.name != "InGameScene") return;
        Initialization();
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
        Debug.Log(player._name.Value +" "+hp+"\n" +UserLoginData.userName.Value+ playerHp.Value);
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
                //HP���������ꍇ�΂�\������
                for (int i = 0; i < hp; i++)
                {
                    _hpSell[i].transform.GetChild(1).gameObject.SetActive(false);
                    _hpSell[i].transform.GetChild(0).gameObject.SetActive(true);

                }
            }
            else
            {
                //HP���������ꍇ�Ԃ�\������
                for (int i = hp; i < _hpSell.Count; i++)
                {
                    Debug.Log("HP��"+i);
                    _hpSell[i].transform.GetChild(0).gameObject.SetActive(false);
                    _hpSell[i].transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            
        }
#endif
    }
    void Dead()
    {
        Debug.Log("���S���܂���");
        GetComponent<PlayerController>().PlayerKilledServerRpc(UserLoginData.userName.Value, GetComponent<PlayerController>()._name.Value);
    }
    // Update is called once per frame
}
