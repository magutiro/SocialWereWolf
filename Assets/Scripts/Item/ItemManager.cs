using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class ItemManager : NetworkBehaviour
{
    public List<ItemController> _items;

    public void AddItem(ItemController _items)
    {
        //引数のItemControllerを_itemsに追加する
    }
    public void RemoveItem(ItemController _items)
    {
        //引数のItemControllerを削除
    }
    public void AllView(bool BW)
    {
        //全てのItemを表示/非表示を切り替える
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.M))
        {

        }
    }
}
