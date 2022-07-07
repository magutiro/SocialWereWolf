using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SQLiteUnity;
using TMPro;
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
    public string WorkName;
    public int RoomId;

    public int ItemId1;
    public int ItemAmout1;
    public int ItemId2;
    public int ItemAmout2;
    public int ItemId3;
    public int ItemAmout3;

    public Type WorkType;
    public State WorkState = State.Impossible;
    
    //必要なアイテムのデータ
    //ItemDictonary<Item,Int>
    public ItemDictonary ItemDic = new ItemDictonary();
    //実際に保持するアイテムデータ
    public ItemDictonary InItemDic = new ItemDictonary();
    public Work(int WorkId, int RoomId,Type type)
    {
        this.WorkId = WorkId;
        this.RoomId = RoomId;
        this.WorkType = type;
    }
    public Work()
    {

    }
    /// <summary>
    /// ワークに必要なアイテムデータを事前にセットする
    /// </summary>
    /// <param name="item"></param>
    public void SetItemDictionary(Item item, int amout){
        ItemDic.SetValue(item, amout);
        ItemDic.Apply();
        InItemDic.SetValue(item, 0);
        InItemDic.Apply();
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

    [SerializeField]
    GameObject WorkPanel;

    [SerializeField]
    TextMeshPro tmpText;

    [SerializeField]
    List<TextMeshPro> workInventryCountText;


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
        return new Work(id, UnityEngine.Random.Range(0, 10),Work.Type.Repair);
    }
    public void SetWorkItemCount(int i, int amout)
    {
        workInventryCountText[i].text = "×" + amout;
    }

    /// <summary>
    /// ワークに必要なアイテムのImageを設定する
    /// </summary>
    /// <param name="work"></param>
    private void SetItemImage(Work work)
    {
        int i = 0;
        for(int j = 0; j < 3; j++)
        {
            ImageList[j].gameObject.SetActive(false);
        }
        foreach (var a in work.ItemDic.GetTable().Values)
        {
            ImageList[i].gameObject.SetActive(true);
            ImageList[i].sprite = (Sprite)Resources.Load(a.ToString());
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
            DailyWorkList.Add(WorkList[UnityEngine.Random.Range(0, WorkList.Count)]);
        }
    }
    /// <summary>
    /// ワークの画面が開かれたときに必要なアイテムなどを表示する
    /// </summary>
    /// <param name="workID"></param>
    public void ViewWork(int workID)
    {
        WorkPanel.SetActive(true);
        tmpText.text = DailyWorkList[workID].WorkName;
        SetItemImage(DailyWorkList[workID]);
    }
    public void CloseWork()
    {
        WorkPanel.SetActive(false);
    }
    /// <summary>
    /// UnityEditorからワークを登録する処理
    /// </summary>
    public void CreateWork()
    {
        SQLiteTable datatable = ConnectSqlite.GetSqliteQuery("Select * from Work");
        if (datatable != null)
        {
            SQLiteTable itemtable = ConnectSqlite.GetSqliteQuery("Select * from Item");
            List<Item> itemList = new List<Item>();
            if(itemtable != null)
            {
                foreach(var row in itemtable.Rows)
                {
                    var id = row["id"];
                    var name = row["name"];
                    Item itemTMP = new Item();
                    itemTMP.id = (int)id;
                    itemTMP.name = name.ToString();

                    itemList.Add(itemTMP);
                }
            }

            foreach (var row in datatable.Rows)
            {
                var id = row["id"];
                var name = row["name"];
                var roomid = row["roomid"];
                var workType = row["workType"];
                var itemid1 = row["itemid1"];
                var itemAmout1 = row["amout1"];


                Work workTMP = new Work((int)id, (int)roomid, (Work.Type)Enum.ToObject(typeof(Work.Type), (int)workType));
                workTMP.WorkName = name.ToString();
                Debug.Log(workTMP.WorkName);
                workTMP.SetItemDictionary(itemList[(int)itemid1], (int)itemAmout1);

                if(row["itemid2"] != null)
                {
                    var itemid2 = row["itemid2"];
                    var itemAmout2 = row["amout2"];
                    workTMP.SetItemDictionary(itemList[(int)itemid2], (int)itemAmout2);
                }
                if (row["itemid3"] != null)
                {
                    var itemid3 = row["itemid3"];
                    var itemAmout3 = row["amout3"];
                    workTMP.SetItemDictionary(itemList[(int)itemid3], (int)itemAmout3);
                }

                WorkList.Add(workTMP);

            }
        }
    }

    public void ImportCsv()
    {
        var items = CsvImporter.ImportItemCsv();
        Debug.Log(Application.streamingAssetsPath);
        ConnectSqlite.SqliteDelete("DELETE FROM Item;");
        foreach (var i in items)
        {
            var id = i.id;
            var name = i.name;
            var type = i.item;
            string sql = "INSERT INTO [Item] VALUES (" + id + ",\"" + name + "\"," + (int)type + ");";
            ConnectSqlite.SqliteInsert(sql);
        }
    }
    public void ImportWorkCSV()
    {
        var woks = CsvImporter.ImportWorkCsv();
        ConnectSqlite.SqliteDelete("DELETE FROM Work;");
        foreach (var w in woks)
        {
            var id = w.WorkId;
            var name = w.WorkName;
            var roomid = w.RoomId;
            var workType = 0;
            var itemid1 = w.ItemId1;
            var itemid2 = w.ItemId2;
            var itemid3 = w.ItemId3;
            var itemAmout1 = w.ItemAmout1;
            var itemAmout2 = w.ItemAmout2;
            var itemAmout3 = w.ItemAmout3;

            string sql = $"INSERT INTO Work (id, name, roomid, workType, itemid1, amout1, itemid2, amout2, itemid3, amout3) VALUES ({id}, '{name}', {roomid},{workType},{itemid1},{itemAmout1},{itemid2},{itemAmout2},{itemid3},{itemAmout3})";
            ConnectSqlite.SqliteInsert(sql);
        }
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
        if (GUILayout.Button("ImportItemToCSV"))
        {
            workManager.ImportCsv();
        }
        if (GUILayout.Button("ImportWorkToCSV"))
        {
            workManager.ImportWorkCSV();
        }
    }
}
#endif
