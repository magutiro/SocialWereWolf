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
        //<int, int> =  <Key, Value> =<ACeID, o»m¦>
        var Itemdic = new Dictionary<int, int>()
        {
            {1, 30},  //ØÞ
            {2, 30},  //Ô
            {3, 30},  //oPc
            {4, 30},  //HÆÉ
            {5, 30},  //B
            {6, 30},  //SÌ
            {7, 30},  //nhK
            {8, 30},  //Tu}VK
            {9, 30},  //ATgCt
            {10, 30}, //XiCp[Ct
            {11, 30}, //8{XR[v
            {12, 30}, //9oe
            {13, 30}, //5.56oe
            /*
            {14, 30}, //ÎY
            {15, 30}, //{
            {16, 30}, //HÆ
            {17, 30}, //Ù¤«
            {18, 30}, //GÐ
            {19, 30}, //V·
            {20, 30}, //Gæ
            {21, 30}, //R¿
            {22, 30}, //¨
            {23, 30}, //¨Ùq
            {24, 30}, //
            {25, 30}, //^I
            {26, 30}, //iCt
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

        // Itemdic ð ItemList Éãü(Directory -> List)
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
        //ItemList Ìgð_Éæ¾@aÉi[
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
/// WFlbNðB·½ßÉp³µÄµÜ¤
/// [System.Serializable]ð­ÌðYêÈ¢
/// </summary>
[System.Serializable]
public class ItemBoxDictonary : Serialize.TableBase<Item, int, ItemBoxDictionaryPair>
{

}

/// <summary>
/// WFlbNðB·½ßÉp³µÄµÜ¤
/// [System.Serializable]ð­ÌðYêÈ¢
/// </summary>
[System.Serializable]
public class ItemBoxDictionaryPair : Serialize.KeyAndValue<Item, int>
{

}
