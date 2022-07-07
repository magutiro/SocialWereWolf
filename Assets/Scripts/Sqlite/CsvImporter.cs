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
        //テキストファイルの読み込みを行ってくれるクラス
        TextAsset textasset = new TextAsset();
        //先ほど用意したcsvファイルを読み込ませる。
        //ファイルは「Resources」フォルダを作り、そこに入れておくこと。また"CSVTestData"の部分はファイル名に合わせて変更する。
        textasset = Resources.Load("Item", typeof(TextAsset)) as TextAsset;
        //CSVSerializerを用いてcsvファイルを配列に流し込む。
        items = CSVSerializer.Deserialize<Item>(textasset.text);

        return items;
    }
    public static Work[] ImportWorkCsv()
    {
        //https://qiita.com/Kirikabu_ueda/items/23b4827abf5b8b6251bc
        Work[] works;
        //テキストファイルの読み込みを行ってくれるクラス
        TextAsset textasset = new TextAsset();
        //先ほど用意したcsvファイルを読み込ませる。
        //ファイルは「Resources」フォルダを作り、そこに入れておくこと。また"CSVTestData"の部分はファイル名に合わせて変更する。
        textasset = Resources.Load("Work", typeof(TextAsset)) as TextAsset;
        //CSVSerializerを用いてcsvファイルを配列に流し込む。
        works = CSVSerializer.Deserialize<Work>(textasset.text);

        return works;
    }
}
