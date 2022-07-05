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
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
/// <summary>
/// �W�F�l���b�N���B�����߂Ɍp�����Ă��܂�
/// [System.Serializable]�������̂�Y��Ȃ�
/// </summary>
[System.Serializable]
public class ItemDictonary : Serialize.TableBase<Item,int,ItemDictionaryPair>
{

}

/// <summary>
/// �W�F�l���b�N���B�����߂Ɍp�����Ă��܂�
/// [System.Serializable]�������̂�Y��Ȃ�
/// </summary>
[System.Serializable]
public class ItemDictionaryPair : Serialize.KeyAndValue<Item, int>
{

}