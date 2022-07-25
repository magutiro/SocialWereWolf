using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Unity.Netcode;

public class NightController : MonoBehaviour
{
    //各UIのパネル
    [SerializeField]
    GameObject MenuPanel;
    [SerializeField]
    GameObject KillPanel;
    [SerializeField]
    GameObject KillPanel2;
    [SerializeField]
    GameObject SkillPanel;
    [SerializeField]
    GameObject SkillPanel2;
    [SerializeField]
    GameObject ItemPanel;
    [SerializeField]
    GameObject ItemPanel2;

    List<GameObject> _panelList;

    public GameObject _panelLootObject; 
    public List<GameObject> _playerButtonList = new List<GameObject>();
    public GameController gm;

    int targetPlayerID;
    int targetItemID;

    PlayerManager playerManager;
    PlayerSkillController player;
    // Start is called before the first frame update
    void Start()
    {
        _panelList = new List<GameObject>() { 
            MenuPanel,
            KillPanel,
            KillPanel2,
            SkillPanel,
            SkillPanel2,
            ItemPanel,
            ItemPanel2,
        };
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        player = playerManager.myPlayer.GetComponent<PlayerSkillController>();

        if(playerManager.myPlayer.GetComponent<PlayerJobState>().playerjob.Value == Job.Dual)
        {
            MenuPanel.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "襲撃する";
        }
        else
        {
            MenuPanel.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "就寝する";
            MenuPanel.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => skip());
        }

        for (int i = 0; i < playerManager.playerList.Count; i++)
        {
            _playerButtonList.Add(_panelLootObject.transform.GetChild(i).gameObject);
            Debug.Log(playerManager.playerList[i].GetComponent<PlayerController>()._name.Value);
            _playerButtonList[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = playerManager.playerList[i].GetComponent<PlayerController>()._name.Value;
        }

        SkillPanel.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = player.skillName;
        switch (player.SkillID)
        {
            case 1:
                //SkillPanel.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>().sprite = (Sprite)Resourse("");
                break;
        }


    }                
    public void skip()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    ///襲撃ボタンを押したときの処理
    ///生存しているデュアル以外を表示する
    /// </summary>
    public void OnKillCommandButton()
    {
        MenuPanel.SetActive(false);
        KillPanel.SetActive(true);
    }
    /// <summary>
    ///襲撃するプレイヤーを選択する処理 
    ///ボタンごとにプレイヤーIDを設定する
    /// </summary>
    /// <param name="playerID"></param>
    public void OnKillSelectButton(int playerID)
    {
        targetPlayerID = playerID-1;
        KillPanel.SetActive(false);
        KillPanel2.SetActive(true);
        KillPanel2.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().SetText(playerManager.playerList[targetPlayerID].GetComponent<PlayerController>()._name.Value);
        //KillPanel2.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = (Sprite)Resources.Load("/Image/Charctor/StandingPict/0"+playerID+"-2.png");
    }
    /// <summary>
    /// 襲撃決定ボタン
    /// </summary>
    public void OnKillAcceptButton()
    {
        //サーバーに襲撃のデータを送信
        RaidPlayerServerRpc(targetPlayerID);
        Debug.Log(targetPlayerID+"を襲撃しました。");
        KillPanel2.SetActive(false);
        MenuPanel.SetActive(true);
    }
    /// <summary>
    /// スキルボタン
    /// </summary>
    public void OnSkillButton()
    {
        MenuPanel.SetActive(false);
        SkillPanel.SetActive(true);
    }
    /// <summary>
    /// スキル使用ボタン
    /// </summary>
    public void OnSkillAcceptButton()
    {
        //サーバーにスキルの使用データを送信
        Debug.Log("スキル使用"); 
        UseSkillServerRpc();
        MenuPanel.SetActive(true);
        SkillPanel.SetActive(false);
    }
    /// <summary>
    /// アイテムボタン
    /// </summary>
    public void OnItemButton()
    {
        MenuPanel.SetActive(false);
        ItemPanel.SetActive(true);
    }
    /// <summary>
    /// アイテム一覧ボタン
    /// </summary>
    public void OnItemSelectButton(int id)
    {
        targetItemID = id;
        ItemPanel.SetActive(false);
        ItemPanel2.SetActive(true);
    }
    /// <summary>
    /// アイテム決定ボタン
    /// </summary>
    public void OnItemUseButton()
    {
        //サーバーにアイテムの使用を送信
        Debug.Log(targetItemID + "のアイテムを使用");
        UseItemServerRpc();
        MenuPanel.SetActive(true);
        ItemPanel2.SetActive(false);
    }
    /// <summary>
    /// キャンセルボタン
    /// </summary>
    /// <param name="hiddenPanel"></param>
    public void OnCancelButton()
    {
        foreach(var p in _panelList.Where(p => p.activeSelf))
        {
            p.SetActive(false);
        }
        MenuPanel.SetActive(true);
    }
    [ServerRpc(RequireOwnership = false)]
    public void RaidPlayerServerRpc(int raidPlayerId)
    {
        gm.RaidPlayer(raidPlayerId);
    }
    [ServerRpc]
    public void UseItemServerRpc()
    {

    }
    [ServerRpc]
    public void UseSkillServerRpc()
    {

    }
}
