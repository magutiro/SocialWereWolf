using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Item
{
    public static string name;
    public static int id;
    public static ItemEnum item;
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
