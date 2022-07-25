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
        //ˆø”‚ÌItemController‚ğ_items‚É’Ç‰Á‚·‚é
    }
    public void RemoveItem(ItemController _items)
    {
        //ˆø”‚ÌItemController‚ğíœ
    }
    public void AllView(bool BW)
    {
        //‘S‚Ä‚ÌItem‚ğ•\¦/”ñ•\¦‚ğØ‚è‘Ö‚¦‚é
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
