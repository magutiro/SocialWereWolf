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
        //������ItemController��_items�ɒǉ�����
    }
    public void RemoveItem(ItemController _items)
    {
        //������ItemController���폜
    }
    public void AllView(bool BW)
    {
        //�S�Ă�Item��\��/��\����؂�ւ���
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
