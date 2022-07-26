using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using SQLiteUnity;

public class InventoryManager : NetworkBehaviour
{
    [SerializeField]
    GameObject iconPrefab = null;
    [SerializeField]
    Transform iconParent = null;
    [SerializeField]
    InventoryItem[] items = null;

    List<Item> itemList;
    // アイテムを持ってるかどうかのフラグ
    bool[] itemFlags;
    int[] itemCount;
    void Start()
    {
        itemFlags = new bool[9];
        itemCount = new int[9];

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
            }
        }
    }
    // アイテムを持ってるかどうかを確認するメソッド
    public bool GetItemFlag(string itemName)
    {
        int index = GetItemIndexFromName(itemName);
        return itemFlags[index];
    }
    public void SetItem(string itemName, bool isOn)
    {
        int index = GetItemIndexFromName(itemName);
        int ItemIndex = 0
            ;
        //インベントリに空きがあるときに、新規アイテムを獲得したとき
        if (index < 0 && isOn && IsItemGet())
        {
            // 新しいアイコンを生成し、インベントリの子に設定
            GameObject icon = Instantiate(iconPrefab, iconParent);
            icon.transform.parent = iconParent;

            for (int b = 0; b < itemFlags.Length; b++)
            {
                if (!itemFlags[b])
                {
                    icon.transform.SetSiblingIndex(b);
                    ItemIndex = b;
                }
            }
            itemFlags[ItemIndex] = true; 
            itemCount[ItemIndex] = 1;
            // アイコンの画像を設定
            icon.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Item/Item" + (index+1));
        }
        // 既に持っているアイテムを獲得したとき
        else if (itemFlags[ItemIndex])
        {
            itemCount[ItemIndex]++;
        }
    }
    public void DeleteItemInInventory(string itemName)
    {
        int index = GetItemIndexFromName(itemName);
        if(index >= 0)
        {

        }
    }
    bool IsItemGet()
    {
        for (int b = 0; b < itemFlags.Length; b++)
        {
            if (!itemFlags[b])
            {
                return true ;
            }
        }
        return false;
    }
    int GetItemIndexFromName(string itemName)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (itemList[i].name == itemName)
            {
                return i;
            }
        }
        Debug.LogWarning("指定されたアイテム名が間違っているか存在しません");
        return -1;
    }
}



// インベントリに登録できるアイテムを定義するためのクラス
[System.Serializable]
public class InventoryItem
{
    public string itemName = "";
    public Sprite itemSprite = null;
}
