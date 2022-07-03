using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    
    public Dictionary<Item, int> ItemDic;
    public Dictionary<Item, int> InItemDic;
    public Work(int WorkId, int RoomId,Type type)
    {
        this.WorkId = WorkId;
        this.RoomId = RoomId;
        this.ItemType = type;
    }

    public void SetItemDictionary(Item item){
        ItemDic.Add(item, ItemDic.Count);
        InItemDic.Add(item, 0);
    }
    public void SetInItem(Item item, int amout)
    {
        InItemDic[item] += amout;
        foreach(var d in ItemDic)
        {
            if(d.Value != InItemDic[d.Key])
            {
                return;
            }
        }
        PossibleWork();
    }
    public void PossibleWork()
    {
        Debug.Log("Š®—¹");
        WorkState = State.Possible;
    }
}

public class WorkManager : MonoBehaviour
{
    public int DailyWorkNum;

    List<Work> WorkList = new List<Work>();
    List<Work> DailyWorkList = new List<Work>();

    public List<Image> ImageList = new List<Image>();

    
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 10; i++)
        {
            WorkList.Add(CreateWork(i));
            WorkList[i].SetItemDictionary(new Item());
            SetItemImage(WorkList[i]);
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
    private void SetItemImage(Work work)
    {
        int i = 0;
        foreach(var a in work.ItemDic.Values)
        {
            ImageList[i].sprite = (Sprite)Resources.Load("");
            i++;
        }
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
