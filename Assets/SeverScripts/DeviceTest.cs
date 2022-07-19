using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using VivoxUnity;

public class DeviceTest : MonoBehaviour
{
    [SerializeField]
    private Text _muteText;

    public VivoxManager _vivox = null;
    // Start is called before the first frame update
#if CLIENT
    void Start()
    {
        _vivox = GameObject.Find("Vivox").GetComponent<VivoxManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnMuteButton()
    {
        //ミュート中なら
        if (_vivox.GetMute())
        {
            //ミュート解除
            if (_vivox.SetMute(false))
            {
                _muteText.text = "ミュート解除中";
            }
        }
        //ミュートされてなかったら
        else
        {
            //ミュートにする
            if (_vivox.SetMute(true))
            {
                _muteText.text = "ミュート中";
            }
        }
    }
#endif
}
