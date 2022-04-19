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
    void Start()
    {
        _vivox = GameObject.Find("PlayerManager").GetComponent<VivoxManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnMuteButton()
    {
        //�~���[�g���Ȃ�
        if (_vivox.GetMute())
        {
            //�~���[�g����
            if (_vivox.SetMute(false))
            {
                _muteText.text = "�~���[�g������";
            }
        }
        //�~���[�g����ĂȂ�������
        else
        {
            //�~���[�g�ɂ���
            if (_vivox.SetMute(true))
            {
                _muteText.text = "�~���[�g��";
            }
        }
    }
}
