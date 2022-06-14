using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public static class UserLoginData
{
    //public static string userName;
    public static NetworkVariable<NetworkString> userName = new NetworkVariable<NetworkString>();
    public static int userID;
}
