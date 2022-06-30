using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Unity.Netcode;

public class NightController : MonoBehaviour
{
    //�eUI�̃p�l��
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
    public List<GameObject> _playerButtonList;
    public GameController gm;

    int targetPlayerID;
    int targetItemID;

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
        for (int i = 0; i < 9; i++)
        {
            _playerButtonList.Add(_panelLootObject.transform.GetChild(i).GetChild(1).gameObject);
            _playerButtonList[i].GetComponent<TextMeshProUGUI>().SetText("PlayerName");
        }
    }                

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    ///�P���{�^�����������Ƃ��̏���
    ///�������Ă���f���A���ȊO��\������
    /// </summary>
    public void OnKillCommandButton()
    {
        MenuPanel.SetActive(false);
        KillPanel.SetActive(true);
    }
    /// <summary>
    ///�P������v���C���[��I�����鏈�� 
    ///�{�^�����ƂɃv���C���[ID��ݒ肷��
    /// </summary>
    /// <param name="playerID"></param>
    public void OnKillSelectButton(int playerID)
    {
        targetPlayerID = playerID;
        KillPanel.SetActive(false);
        KillPanel2.SetActive(true);
        KillPanel2.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().SetText("PlayerName");
        //KillPanel2.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = (Sprite)Resources.Load("/Image/Charctor/StandingPict/0"+playerID+"-2.png");
    }
    /// <summary>
    /// �P������{�^��
    /// </summary>
    public void OnKillAcceptButton()
    {
        //�T�[�o�[�ɏP���̃f�[�^�𑗐M
        RaidPlayerServerRpc();
        Debug.Log(targetPlayerID+"���P�����܂����B");
        KillPanel2.SetActive(false);
        MenuPanel.SetActive(true);
    }
    /// <summary>
    /// �X�L���{�^��
    /// </summary>
    public void OnSkillButton()
    {
        MenuPanel.SetActive(false);
        SkillPanel.SetActive(true);
    }
    /// <summary>
    /// �X�L���g�p�{�^��
    /// </summary>
    public void OnSkillAcceptButton()
    {
        //�T�[�o�[�ɃX�L���̎g�p�f�[�^�𑗐M
        Debug.Log("�X�L���g�p"); 
        UseSkillServerRpc();
        MenuPanel.SetActive(true);
        SkillPanel.SetActive(false);
    }
    /// <summary>
    /// �A�C�e���{�^��
    /// </summary>
    public void OnItemButton()
    {
        MenuPanel.SetActive(false);
        ItemPanel.SetActive(true);
    }
    /// <summary>
    /// �A�C�e���ꗗ�{�^��
    /// </summary>
    public void OnItemSelectButton(int id)
    {
        targetItemID = id;
        ItemPanel.SetActive(false);
        ItemPanel2.SetActive(true);
    }
    /// <summary>
    /// �A�C�e������{�^��
    /// </summary>
    public void OnItemUseButton()
    {
        //�T�[�o�[�ɃA�C�e���̎g�p�𑗐M
        Debug.Log(targetItemID + "�̃A�C�e�����g�p");
        UseItemServerRpc();
        MenuPanel.SetActive(true);
        ItemPanel2.SetActive(false);
    }
    /// <summary>
    /// �L�����Z���{�^��
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
    [ServerRpc]
    public void RaidPlayerServerRpc()
    {
        gm.RaidPlayer(targetPlayerID);
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
