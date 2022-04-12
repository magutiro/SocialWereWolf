using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WebSocketSharp;
using System;

public class WebSocketClientManager
{
    public static WebSocket webSocket;
    public static UnityAction<Dictionary<string, PlayerActionData>> recieveCompletedHandler;

    /// <summary>
    /// WebSocket�ڑ�
    /// </summary>
    public static void Connect()
    {
        if (webSocket == null)
        {

#if UNITY_ANDROID
            webSocket = new WebSocket("ws://119.242.252.212:4115/");
#else
            webSocket = new WebSocket("ws://localhost:4115/");
#endif
            webSocket.OnOpen += (sender, e) => { Debug.Log("open"); };
            webSocket.OnError += (sender, e) => { Debug.Log("miss"); }; 
            webSocket.OnMessage += (sender, e) => RecieveAllPlayerAction(e.Data);
            webSocket.Connect();
        }
    }

    private static void RecieveAllUserAction(string data)
    {
        //throw new NotImplementedException();
    }

    /// <summary>
    /// WebSocket�ؒf
    /// </summary>
    public static void DisConnect()
    {
        webSocket.Close();
        webSocket = null;
    }

    /// <summary>
    /// WebSocket���M
    /// </summary>
    /// <param name="action"></param>
    /// <param name="pos"></param>
    /// <param name="way"></param>
    /// <param name="range"></param>
    public static void SendPlayerAction(string action, Vector3 pos, string way, float range)
    {
        var userActionData = new PlayerActionData
        {
            action = action,
            way = way,
            room_no = 1,
            user = UserLoginData.userName,
            pos_x = pos.x,
            pos_y = pos.y,
            pos_z = pos.z,
            range = range
        };

        webSocket.Send(userActionData.ToJson());
    }

    /// <summary>
    /// WebSocket��M
    /// </summary>
    /// <param name="json"></param>
    public static void RecieveAllPlayerAction(string json)
    {
        var allUserActionHash = PlayerActionData.FromJson(json, 1);
        recieveCompletedHandler?.Invoke(allUserActionHash);
    }
}