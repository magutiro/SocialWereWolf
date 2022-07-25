using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine;

public class RandomSpawner : NetworkBehaviour
{
    // �I�u�W�F�N�g���i�[����ϐ�
    public GameObject respawnPrefab;
    //�I�u�W�F�N�g�̐����ʒu���i�[����z��
    public GameObject[] spawnPoints;
    // �I�u�W�F�N�g�̐����ʒu���i�[����ϐ�
    public Vector3 spawnPoint;
    // �O��̃V�[���Ő��������I�u�W�F�N�g�̐����ʒu���i�[����ϐ�
    // �V�[���������[�h�����ۂɃI�u�W�F�N�g�̐����ʒu���O��Ɠ�������r���邽�߂̕ϐ�
    public static Vector3 RespawnPoint;

    public List<int> Itembox;
    public int popItem;
    public int Id;

    [ServerRpc]
    public void PlacementItemServerRpc()
    {
        var prefab = Instantiate(respawnPrefab, spawnPoint, transform.rotation);
        
        prefab.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("UI/Item/Item"�@+ Id);
        prefab?.GetComponent<NetworkObject>().Spawn(); //�m
        Debug.Log("�T�[�o�[");
    } 

    public void RamSpoawn()
    {
        ItemBox itembox = GetComponent<ItemBox>();
        Itembox =itembox.ItemList;
        popItem = Itembox[Random.Range(0, Itembox.Count)];

        Id = popItem;
        Debug.Log(Id);
        
        // �^�O Respawn ���ݒ肳�ꂽ�I�u�W�F�N�g��z�� spawnPoints�Ɋi�[
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

        // �z�� spawnPoints ����łȂ�0���傫���ꍇ
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            // �I�u�W�F�N�g�̐����ʒu���O��̃V�[���Ɠ����ꍇ�͏������J��Ԃ�
            PlacementItemServerRpc();
            // �I�u�W�F�N�g�̐����ʒu���r�p�̕ϐ��Ɋi�[����
            RespawnPoint = spawnPoint;
        }
        else
        {
            Debug.Log("���X�|�[���|�C���g�G���[");
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
