using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine;

public class RandomSpawner : NetworkBehaviour
{
    // オブジェクトを格納する変数
    public GameObject respawnPrefab;
    //オブジェクトの生成位置を格納する配列
    public GameObject[] spawnPoints;
    // オブジェクトの生成位置を格納する変数
    public Vector3 spawnPoint;
    // 前回のシーンで生成したオブジェクトの生成位置を格納する変数
    // シーンをリロードした際にオブジェクトの生成位置が前回と同じか比較するための変数
    public static Vector3 RespawnPoint;

    public List<int> Itembox;
    public int popItem;
    public int Id;

    [ServerRpc]
    public void PlacementItemServerRpc()
    {
        var prefab = Instantiate(respawnPrefab, spawnPoint, transform.rotation);
        
        prefab.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("UI/Item/Item"　+ Id);
        prefab?.GetComponent<NetworkObject>().Spawn(); //確
        Debug.Log("サーバー");
    } 

    public void RamSpoawn()
    {
        ItemBox itembox = GetComponent<ItemBox>();
        Itembox =itembox.ItemList;
        popItem = Itembox[Random.Range(0, Itembox.Count)];

        Id = popItem;
        Debug.Log(Id);
        
        // タグ Respawn が設定されたオブジェクトを配列 spawnPointsに格納
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

        // 配列 spawnPoints が空でなく0より大きい場合
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            // オブジェクトの生成位置が前回のシーンと同じ場合は処理を繰り返す
            PlacementItemServerRpc();
            // オブジェクトの生成位置を比較用の変数に格納する
            RespawnPoint = spawnPoint;
        }
        else
        {
            Debug.Log("リスポーンポイントエラー");
        }

    }

    public void OnSpoawn()
    {
        RamSpoawn();
        Debug.Log("OnSpawn");
    } 

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
