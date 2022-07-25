using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class InventoryManager : NetworkBehaviour
{


    [SerializeField]
    GameObject iconPrefab = null;
    [SerializeField]
    Transform iconParent = null;
    [SerializeField]
    InventoryItem[] items = null;
    // �A�C�e���������Ă邩�ǂ����̃t���O
    bool[] itemFlags;
    // �A�C�e���̃A�C�R�����Ǘ����邽�߂̃f�B�N�V���i��
    Dictionary<int, GameObject> icons = new Dictionary<int, GameObject>();
    void Start()
    {
        itemFlags = new bool[items.Length];
    }
    // �A�C�e���������Ă邩�ǂ������m�F���郁�\�b�h
    public bool GetItemFlag(string itemName)
    {
        int index = GetItemIndexFromName(itemName);
        return itemFlags[index];
    }
    public void SetItem(string itemName, bool isOn)
    {
        int index = GetItemIndexFromName(itemName);
        if (!itemFlags[index] && isOn)
        {
            // �A�C�e���������̏�ԂŐV�������肵���Ƃ�
            // �V�����A�C�R���𐶐����A�C���x���g���̃L�����o�X�̎q�ɐݒ�
            GameObject icon = Instantiate(iconPrefab, iconParent);
            // �A�C�R���̉摜��ݒ�
            icon.GetComponent<Image>().sprite = items[index].itemSprite;
            icons.Add(index, icon);
        }
        else if (itemFlags[index] && !isOn)
        {
            // �A�C�e���������ɍ폜����Ƃ�
            GameObject icon = icons[index];
            // �A�C�e���̃A�C�R�����폜
            Destroy(icon);
            // �A�C�R���̃f�B�N�V���i������Ώۂ̃A�C�e�����폜
            icons.Remove(index);
        }
        itemFlags[index] = isOn;
    }
    int GetItemIndexFromName(string itemName)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].itemName == itemName)
            {
                return i;
            }
        }
        Debug.LogWarning("�w�肳�ꂽ�A�C�e�������Ԉ���Ă��邩���݂��܂���");
        return 0;
    }



// �C���x���g���ɓo�^�ł���A�C�e�����`���邽�߂̃N���X
[System.Serializable]
public class InventoryItem
{
    public string itemName = "";
    public Sprite itemSprite = null;
}

/*public int MAX_ITEMS = 9;
    public List<ItemController> _myInventory;
    public List<Image> _image;  //�C���x���g���̊e�C���[�W������

    public void GetItem(ItemController _myInventory)
    {
        //�����̃A�C�e�����C���x���g���ɉ�����
    }
    public void DropItem(int MAX_ITEMS)
    {
        //�����̔ԍ��ɂ���A�C�e����n�ʂɃh���b�v����
    }
    public void RemoveItem(int MAX_ITEMS)
    {
        //�����̔ԍ��ɂ���A�C�e������������
    }
    public void DropItem(ItemController _myInventory)
    {
        //�����̃A�C�e�����C���x���g���ɂ���΃h���b�v����
    }
    public void RemoveItem(ItemController _myInventory)
    {
        //�����̃A�C�e�����C���x���g���ɂ���Ώ�������
    }
    public void ViewItem()
    {
        //_myInventor�ɂ���A�C�e�����C���x���g���ɕ\��������
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/
}
