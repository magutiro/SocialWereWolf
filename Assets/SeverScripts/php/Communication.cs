using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Communication : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(cs());
    }
    IEnumerator cs()
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get("http://52.193.191.245/Develop/WereWolf/public/name?user_id=4b3403665fea6&user_name=maguro");

        yield return unityWebRequest.SendWebRequest();
        if (!string.IsNullOrEmpty(unityWebRequest.error))
        {
            UnityEngine.Debug.LogError(unityWebRequest.error);
        }
        string text = unityWebRequest.downloadHandler.text;
        Debug.Log(text);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
