using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[System.SerializableAttribute]
public class Item
{
    public string name;
    public int id;
    public ItemEnum item;
    public enum ItemEnum
    {
        Field,
        MyHand,
    }
}
public class ItemController : NetworkBehaviour
{
    public Item item;

    InventoryManager inventoryManager;
    void Start()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }

    void Update()
    {
        
    }

    public void OnUseButton()
    {
        inventoryManager.SetItem(item.name, true) ;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {

        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        
    }
}
/// <summary>
/// ジェネリックを隠すために継承してしまう
/// [System.Serializable]を書くのを忘れない
/// </summary>
[System.Serializable]
public class ItemDictonary : Serialize.TableBase<Item,int,ItemDictionaryPair>
{

}

/// <summary>
/// ジェネリックを隠すために継承してしまう
/// [System.Serializable]を書くのを忘れない
/// </summary>
[System.Serializable]
public class ItemDictionaryPair : Serialize.KeyAndValue<Item, int>
{

}