using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.SerializableAttribute]
public class Work
{
    public enum Type
    {
        Task,
        Repair
    }
    public enum State
    {
        Possible,
        Impossible
    }
    public int WorkId;
    public int RoomId;
    public Type ItemType;
    public State WorkState = State.Impossible;
    public string WorkName;
    
    //�K�v�ȃA�C�e���̃f�[�^
    //ItemDictonary<Item,Int>
    public ItemDictonary ItemDic;
    //���ۂɕێ�����A�C�e���f�[�^
    public ItemDictonary InItemDic;
    public Work(int WorkId, int RoomId,Type type)
    {
        this.WorkId = WorkId;
        this.RoomId = RoomId;
        this.ItemType = type;
    }
    /// <summary>
    /// ���[�N�ɕK�v�ȃA�C�e���f�[�^�����O�ɃZ�b�g����
    /// </summary>
    /// <param name="item"></param>
    public void SetItemDictionary(Item item, int amout){
        ItemDic.Add(item, amout);
        InItemDic.Add(item, 0);
        Debug.Log(item.name);
    }
    /// <summary>
    /// �A�C�e����[�i����Ƃ��Ƀ��[�N���������邩�ǂ����m�F����
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amout"></param>
    public void SetInItem(Item item, int amout)
    {
        //�A�C�e����[�i����
        InItemDic.GetTable()[item] += amout;
        foreach(var d in ItemDic.GetTable())
        {
            //�����A�C�e���̌����قȂ邩�ǂ�������B�قȂ�ΏI��
            if(d.Value != InItemDic.GetTable()[d.Key])
            {
                return;
            }
        }
        //���[�N�����������Ƃ��̂ݏ���
        PossibleWork();
    }
    /// <summary>
    /// ���[�N�����������Ƃ��̏���
    /// </summary>
    public void PossibleWork()
    {
        Debug.Log("����");
        WorkState = State.Possible;
    }
}

public class WorkManager : MonoBehaviour
{
    public int DailyWorkNum;

    public List<Work> WorkList = new List<Work>();
    List<Work> DailyWorkList = new List<Work>();

    public List<Image> ImageList = new List<Image>();

    /// <summary>
    /// Unity�̃G�f�B�^���烏�[�N��o�^����Ƃ��Ɏg�p����ϐ�
    /// </summary>
#if UNITY_EDITOR
    public Work TMPWork;
    public List<int> listIndex;
    public List<Item> itemList;
    public List<int> amoutList;
#endif
    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }

    Work CreateWork(int id)
    {
        return new Work(id, Random.Range(0, 10),Work.Type.Repair);
    }
    /// <summary>
    /// ���[�N�ɕK�v�ȃA�C�e����Image��ݒ肷��
    /// </summary>
    /// <param name="work"></param>
    private void SetItemImage(Work work)
    {
        int i = 0;
        foreach (var a in work.ItemDic.GetTable().Values)
        {
            ImageList[i].sprite = (Sprite)Resources.Load("");
            i++;
        }
    }
    /// <summary>
    /// �����̃f�C���[���[�N��ݒ肷��
    /// </summary>
    public void AddWork()
    {
        DailyWorkList = new List<Work>();
        for (int w = 0; w < DailyWorkNum; w++)
        {
            DailyWorkList.Add(WorkList[Random.Range(0, 10)]);
        }
    }
    /// <summary>
    /// ���[�N�̉�ʂ��J���ꂽ�Ƃ��ɕK�v�ȃA�C�e���Ȃǂ�\������
    /// </summary>
    /// <param name="workID"></param>
    public void ViewWork(int workID)
    {
        SetItemImage(WorkList[workID]);
    }
    /// <summary>
    /// UnityEditor���烏�[�N��o�^���鏈��
    /// </summary>
    public void CreateWork()
    {
        WorkList.Add(TMPWork);
        int i=0;
        foreach(var a in listIndex)
        {
            WorkList[WorkList.Count - 1].SetItemDictionary(itemList[a], amoutList[++i]) ;
        }
        Debug.Log(TMPWork.WorkName+"�̃��[�N���ǉ�����܂����B");
        TMPWork = null;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(WorkManager))]
public class WorkCreateEditer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        WorkManager workManager = target as WorkManager;

        if (GUILayout.Button("CreateWork"))
        {
            workManager.CreateWork();
        }
    }
}
#endif
