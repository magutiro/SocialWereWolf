using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemBox : NetworkBehaviour
{
    public List<int> ItemList;

    public void Dictionary()
    {
        //<int, int> =  <Key, Value> =<�A�C�e��ID, �o���m��>
        var Itemdic = new Dictionary<int, int>()
        {
            {1, 30},  //�؍�
            {2, 30},  //����
            {3, 30},  //�o�P�c
            {4, 30},  //�H�ƌ�
            {5, 30},  //�B
            {6, 30},  //�S�̌�
            {7, 30},  //�n���h�K��
            {8, 30},  //�T�u�}�V���K��
            {9, 30},  //�A�T���g���C�t��
            {10, 30}, //�X�i�C�p�[���C�t��
            {11, 30}, //8�{�X�R�[�v
            {12, 30}, //9�o�e
            {13, 30}, //5.56�o�e
            {14, 30}, //�ΒY
            {15, 30}, //�{
            {16, 30}, //�H��
            {17, 30}, //�ق���
            {18, 30}, //�G��
            {19, 30}, //�V����
            {20, 30}, //�G��
            {21, 30}, //�R��
            {22, 30}, //����
            {23, 30}, //�����َq
            {24, 30}, //��
            {25, 30}, //�^�I��
            {26, 30}, //�i�C�t
        };

        // Itemdic �� ItemList �ɑ��(Directory -> List)
        ItemList = new List<int>(Itemdic.Keys);
}

    public void ItemLis()
    {
        //ItemList �̒��g�������_���Ɏ擾�@a�Ɋi�[
        int a = ItemList[Random.Range(0, ItemList.Count)];
        Debug.Log(a);
    }

    internal static GameObject Dictionary(int v, object a)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        Dictionary();
        ItemLis();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
