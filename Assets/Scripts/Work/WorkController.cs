using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
public class WorkController : MonoBehaviour
{
    public WorkManager workManager;
    public Work work;

    bool isPlayerHit;

    public int _workID;

    Vector2 mousePos = Vector2.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        workManager = transform.root.GetComponent<WorkManager>();
        mousePos = GameObject.Find("EventSystem").gameObject.GetComponent<PlayerInput>().currentActionMap["MousePos"].ReadValue<Vector2>();
        
        GetWork();
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (EventSystem.current.IsPointerOverGameObject()) return;
#else
   if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))return;
#endif
        if (isPlayerHit && Mouse.current.leftButton.wasPressedThisFrame)
        {

            // Rayを発射
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 1));
            RaycastHit2D hit2d = Physics2D.Raycast(mousePos, (Vector2)ray.direction);

            // Rayで何もヒットしなかったら画面タッチイベント関数を呼ぶ
            if (hit2d)
            {
                Debug.Log(hit2d.transform.gameObject.name);
            }
            if (!hit2d)
            {
                workManager.CloseWork();
                isPlayerHit = false;

            }
        }
    }
    public void GetWork()
    {
        var works = workManager.DailyWorkList.Where(w => w.RoomId == _workID);
        foreach(var w in works)
        {
            Debug.Log(w.WorkName);
            work = w;
        }
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("OtherPlayer"))
        {
            isPlayerHit = true;
        }
        
    }
    public void OnUseButton()
    {
        if (isPlayerHit)
        {
            Debug.Log("ワーク開始");
            workManager.ViewWork(work.WorkId);
        }
    }
}
