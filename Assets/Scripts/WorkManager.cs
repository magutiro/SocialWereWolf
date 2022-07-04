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
    
    //必要なアイテムのデータ
    //ItemDictonary<Item,Int>
    public ItemDictonary ItemDic;
    //実際に保持するアイテムデータ
    public ItemDictonary InItemDic;
    public Work(int WorkId, int RoomId,Type type)
    {
        this.WorkId = WorkId;
        this.RoomId = RoomId;
        this.ItemType = type;
    }
    /// <summary>
    /// ワークに必要なアイテムデータを事前にセットする
    /// </summary>
    /// <param name="item"></param>
    public void SetItemDictionary(Item item, int amout){
        ItemDic.Add(item, amout);
        InItemDic.Add(item, 0);
        Debug.Log(item.name);
    }
    /// <summary>
    /// アイテムを納品するときにワークが完了するかどうか確認する
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amout"></param>
    public void SetInItem(Item item, int amout)
    {
        //アイテムを納品する
        InItemDic.GetTable()[item] += amout;
        foreach(var d in ItemDic.GetTable())
        {
            //同じアイテムの個数が異なるかどうか判定。異なれば終了
            if(d.Value != InItemDic.GetTable()[d.Key])
            {
                return;
            }
        }
        //ワークが完了したときのみ処理
        PossibleWork();
    }
    /// <summary>
    /// ワークが完了したときの処理
    /// </summary>
    public void PossibleWork()
    {
        Debug.Log("完了");
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
    /// Unityのエディタからワークを登録するときに使用する変数
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
    /// ワークに必要なアイテムのImageを設定する
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
    /// 毎日のデイリーワークを設定する
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
    /// ワークの画面が開かれたときに必要なアイテムなどを表示する
    /// </summary>
    /// <param name="workID"></param>
    public void ViewWork(int workID)
    {
        SetItemImage(WorkList[workID]);
    }
    /// <summary>
    /// UnityEditorからワークを登録する処理
    /// </summary>
    public void CreateWork()
    {
        WorkList.Add(TMPWork);
        int i=0;
        foreach(var a in listIndex)
        {
            WorkList[WorkList.Count - 1].SetItemDictionary(itemList[a], amoutList[++i]) ;
        }
        Debug.Log(TMPWork.WorkName+"のワークが追加されました。");
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
