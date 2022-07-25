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
    public Vector2 spawnPoint;
    // �O��̃V�[���Ő��������I�u�W�F�N�g�̐����ʒu���i�[����ϐ�
    // �V�[���������[�h�����ۂɃI�u�W�F�N�g�̐����ʒu���O��Ɠ�������r���邽�߂̕ϐ�
    public static Vector3 RespawnPoint;

    public List<int> Itembox;
    public int popItem;
    public int Id;

    ItemBox itembox;

    [ServerRpc]
    public void PlacementItemServerRpc()
    {
        var prefab = Instantiate(respawnPrefab, spawnPoint, transform.rotation);

        prefab.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("UI/Item/Item" + (Id + 1));
        prefab.GetComponent<ItemController>().item = itembox.itemList[Id];
        prefab?.GetComponent<NetworkObject>().Spawn(); //�m
        Debug.Log("�T�[�o�[");
    } 

    public void RamSpoawn()
    {
        
        // �^�O Respawn ���ݒ肳�ꂽ�I�u�W�F�N�g��z�� spawnPoints�Ɋi�[
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        for(int p = 0; p < spawnPoints.Length; p++)
        {
            itembox = spawnPoints[p].GetComponent<ItemBox>();
            
            Itembox = itembox.ItemList;

            int sum = 0;
            List<int> boxPercent = new List<int>(new int[itembox.itemBoxDictonary.GetTable().Count]);
            Debug.Log(itembox.itemBoxDictonary.GetTable().Count);
            List<int> itemPercent = new List<int>(new int[itembox.itemBoxDictonary.GetTable().Count]);
            foreach(var item in itembox.itemBoxDictonary.GetTable())
            {
                sum += item.Value;
                boxPercent[item.Key.id-1] = item.Value;
                itemPercent[item.Key.id-1] = item.Value * item.Key.id;
            }

            int random = Random.Range(0, sum);
            for(int i = 0; i < itemPercent.Count; i++)
            {
                if(random < itemPercent[i])
                {
                    popItem = i;
                    break;
                }
            }


            Id = popItem;
            Debug.Log("�A�C�e��" + Id);

            // �z�� spawnPoints ����łȂ�0���傫���ꍇ
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[p].transform.position;
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


    }

    public void OnSpoawn()
    {
        //RamSpoawn();
        Debug.Log("OnSpawn");
    } 

    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
            itembox = GetComponent<ItemBox>();
            RamSpoawn();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
