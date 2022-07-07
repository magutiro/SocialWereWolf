using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// Imageコンポーネントを必要とする
[RequireComponent(typeof(Image))]
public class ItemDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    // ドラッグ前の位置
    private Vector3 prevPos;

    //基準点（マウスの基準は左下だが、オブジェクトの基準は画面中央になるので補正する。）
    private Vector2 rootPos;

    [SerializeField]
    WorkManager workManager;

    // Use this for initialization
    void Start()
    {
        rootPos = new Vector3(400f, 0, 0f); //画面の半分（400, 300）
        prevPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }


    //ドラッグ＆ドロップ関係

    public void OnBeginDrag(PointerEventData eventData)
    {
        // ドラッグ前の位置を記憶しておく
        prevPos = transform.localPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ドラッグ中は位置を更新する
        transform.position = eventData.position;
        //Debug.Log("eventData.position: " + eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // ドラッグ前の位置に戻す
        transform.localPosition = prevPos;
        //transform.position = eventData.position;


    }

    public void OnDrop(PointerEventData eventData)
    {
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        
        foreach (var hit in raycastResults)
        {
            // もし DroppableField の上なら、その位置に固定する
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
