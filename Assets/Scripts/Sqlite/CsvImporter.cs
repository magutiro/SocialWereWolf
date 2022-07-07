using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CsvImporter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static Item[] ImportItemCsv()
    {
        //https://qiita.com/Kirikabu_ueda/items/23b4827abf5b8b6251bc
        Item[] items;
        //�e�L�X�g�t�@�C���̓ǂݍ��݂��s���Ă����N���X
        TextAsset textasset = new TextAsset();
        //��قǗp�ӂ���csv�t�@�C����ǂݍ��܂���B
        //�t�@�C���́uResources�v�t�H���_�����A�����ɓ���Ă������ƁB�܂�"CSVTestData"�̕����̓t�@�C�����ɍ��킹�ĕύX����B
        textasset = Resources.Load("Item", typeof(TextAsset)) as TextAsset;
        //CSVSerializer��p����csv�t�@�C����z��ɗ������ށB
        items = CSVSerializer.Deserialize<Item>(textasset.text);

        return items;
    }
    public static Work[] ImportWorkCsv()
    {
        //https://qiita.com/Kirikabu_ueda/items/23b4827abf5b8b6251bc
        Work[] works;
        //�e�L�X�g�t�@�C���̓ǂݍ��݂��s���Ă����N���X
        TextAsset textasset = new TextAsset();
        //��قǗp�ӂ���csv�t�@�C����ǂݍ��܂���B
        //�t�@�C���́uResources�v�t�H���_�����A�����ɓ���Ă������ƁB�܂�"CSVTestData"�̕����̓t�@�C�����ɍ��킹�ĕύX����B
        textasset = Resources.Load("Work", typeof(TextAsset)) as TextAsset;
        //CSVSerializer��p����csv�t�@�C����z��ɗ������ށB
        works = CSVSerializer.Deserialize<Work>(textasset.text);

        return works;
    }
}
