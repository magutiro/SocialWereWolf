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
    // �A�C�e���������Ă邩�ǂ����̃t���O
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
    // �A�C�e���������Ă邩�ǂ������m�F���郁�\�b�h
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
        //�C���x���g���ɋ󂫂�����Ƃ��ɁA�V�K�A�C�e�����l�������Ƃ�
        if (index < 0 && isOn && IsItemGet())
        {
            // �V�����A�C�R���𐶐����A�C���x���g���̎q�ɐݒ�
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
            // �A�C�R���̉摜��ݒ�
            icon.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Item/Item" + (index+1));
        }
        // ���Ɏ����Ă���A�C�e�����l�������Ƃ�
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
        Debug.LogWarning("�w�肳�ꂽ�A�C�e�������Ԉ���Ă��邩���݂��܂���");
        return -1;
    }
}



// �C���x���g���ɓo�^�ł���A�C�e�����`���邽�߂̃N���X
[System.Serializable]
public class InventoryItem
{
    public string itemName = "";
    public Sprite itemSprite = null;
}
