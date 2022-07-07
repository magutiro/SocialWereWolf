using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// Image�R���|�[�l���g��K�v�Ƃ���
[RequireComponent(typeof(Image))]
public class ItemDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    // �h���b�O�O�̈ʒu
    private Vector3 prevPos;

    //��_�i�}�E�X�̊�͍��������A�I�u�W�F�N�g�̊�͉�ʒ����ɂȂ�̂ŕ␳����B�j
    private Vector2 rootPos;

    [SerializeField]
    WorkManager workManager;

    // Use this for initialization
    void Start()
    {
        rootPos = new Vector3(400f, 0, 0f); //��ʂ̔����i400, 300�j
        prevPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }


    //�h���b�O���h���b�v�֌W

    public void OnBeginDrag(PointerEventData eventData)
    {
        // �h���b�O�O�̈ʒu���L�����Ă���
        prevPos = transform.localPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // �h���b�O���͈ʒu���X�V����
        transform.position = eventData.position;
        //Debug.Log("eventData.position: " + eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // �h���b�O�O�̈ʒu�ɖ߂�
        transform.localPosition = prevPos;
        //transform.position = eventData.position;


    }

    public void OnDrop(PointerEventData eventData)
    {
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        
        foreach (var hit in raycastResults)
        {
            // ���� DroppableField �̏�Ȃ�A���̈ʒu�ɌŒ肷��
            if (hit.gameObject.CompareTag("DroppableField"))
            {
                transform.position = hit.gameObject.transform.position;
                transform.parent = hit.gameObject.transform;
                this.enabled = false;
                workManager.SetWorkItemCount(0,1);
            }
        }
    }
}
