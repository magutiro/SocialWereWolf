using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Work
{
    public enum Type
    {
        Task,
        Repair
    }
    public int WorkId;
    public int RoomId;
    public Type ItemType;
    public string WorkName;
    
    public Dictionary<Item, int> ItemDic;
    public Work(int WorkId, int RoomId,Type type)
    {
        this.WorkId = WorkId;
        this.RoomId = RoomId;
        this.ItemType = type;
    }

    public void SetItemDictionary(Item item){
        ItemDic.Add(item, ItemDic.Count);
    }
}

public class WorkManager : MonoBehaviour
{
    public int DailyWorkNum;

    List<Work> WorkList = new List<Work>();
    List<Work> DailyWorkList = new List<Work>();
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 10; i++)
        {
            WorkList.Add(CreateWork(i));
            WorkList[i].SetItemDictionary(new Item());
        }
    }

    Work CreateWork(int id)
    {
        return new Work(id, Random.Range(0, 10),Work.Type.Repair);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddWork()
    {
        WorkList = new List<Work>();
        for (int w = 0; w < DailyWorkNum; w++)
        {
            DailyWorkList.Add(WorkList[Random.Range(0, 10)]);
        }
    }
}