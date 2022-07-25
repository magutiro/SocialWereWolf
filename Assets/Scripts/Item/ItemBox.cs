using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
using SQLiteUnity;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ItemBox : NetworkBehaviour
{
    public List<int> ItemList = new List<int>();
    public List<Item> itemList = new List<Item>();

    public ItemBoxDictonary itemBoxDictonary = new ItemBoxDictonary();

    public void SetDictionary()
    {
        ResetDic();
        //<int, int> =  <Key, Value> =<アイテムID, 出現確率>
        var Itemdic = new Dictionary<int, int>()
        {
            {1, 30},  //木材
            {2, 30},  //歯車
            {3, 30},  //バケツ
            {4, 30},  //食糧庫
            {5, 30},  //釘
            {6, 30},  //鉄の剣
            {7, 30},  //ハンドガン
            {8, 30},  //サブマシンガン
            {9, 30},  //アサルトライフル
            {10, 30}, //スナイパーライフル
            {11, 30}, //8倍スコープ
            {12, 30}, //9㎜弾
            {13, 30}, //5.56㎜弾
            /*
            {14, 30}, //石炭
            {15, 30}, //本
            {16, 30}, //食糧
            {17, 30}, //ほうき
            {18, 30}, //雑巾
            {19, 30}, //新聞紙
            {20, 30}, //絵画
            {21, 30}, //燃料
            {22, 30}, //お茶
            {23, 30}, //お茶菓子
            {24, 30}, //服
            {25, 30}, //タオル
            {26, 30}, //ナイフ
            */
        };

        SQLiteTable itemtable = ConnectSqlite.GetSqliteQuery("Select * from Item");
        if (itemtable != null)
        {
            foreach (var row in itemtable.Rows)
            {
                var id = row["id"];
                var name = row["name"];
                Item itemTMP = new Item();
                itemTMP.id = (int)id;
                itemTMP.name = name.ToString();
                itemList.Add(itemTMP);
                ItemList.Add((int)id);

                itemBoxDictonary.SetValue(itemTMP, Itemdic[(int)id]);
                itemBoxDictonary.Apply();
            }
        }

        // Itemdic を ItemList に代入(Directory -> List)
        //ItemList = new List<int>(Itemdic.Keys);
    }
    public void ResetDic()
    {
        ItemList = new List<int>();
        itemList = new List<Item>();

        itemBoxDictonary = new ItemBoxDictonary();
    }
    public void ItemLis()
    {
        //ItemList の中身をランダムに取得　aに格納
        int a = ItemList[Random.Range(0, ItemList.Count)];
        Debug.Log(a);
    }
    

    // Start is called before the first frame update
    void Start()
    {
        ItemLis();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(ItemBox))]
public class ItemBoxEditer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ItemBox itemBox = target as ItemBox;

        if (GUILayout.Button("ItemBoxSetDictionary"))
        {
            itemBox.ResetDic();
            itemBox.SetDictionary();
        }
    }
}
#endif



/// <summary>
/// ジェネリックを隠すために継承してしまう
/// [System.Serializable]を書くのを忘れない
/// </summary>
[System.Serializable]
public class ItemBoxDictonary : Serialize.TableBase<Item, int, ItemBoxDictionaryPair>
{

}

/// <summary>
/// ジェネリックを隠すために継承してしまう
/// [System.Serializable]を書くのを忘れない
/// </summary>
[System.Serializable]
public class ItemBoxDictionaryPair : Serialize.KeyAndValue<Item, int>
{

}
