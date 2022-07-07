using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class WorkController : MonoBehaviour
{
    WorkManager workManager;
    Work work;

    bool isPlayerHit;

    public int _workID;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerHit && Input.GetMouseButtonDown(0))
        {

            // Rayを発射
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit2d = Physics2D.Raycast((Vector2)Input.mousePosition, (Vector2)ray.direction);

            // Rayで何もヒットしなかったら画面タッチイベント関数を呼ぶ
            if (hit2d.transform.gameObject.name != "WorkPanel")
            {
                workManager.CloseWork();
            }
        }
    }
    public void GetWork()
    {
        var works = workManager.WorkList.Where(w => w.RoomId == _workID);
        foreach(var w in works)
        {
            work = w;
        }
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            isPlayerHit = true;
        }
    }
    public void OnUseButton()
    {
        if (isPlayerHit)
        {
            workManager.ViewWork(work.WorkId);
        }
    }
}
