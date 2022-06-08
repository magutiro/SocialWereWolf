using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LambdaJsonData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable] 
public class Lambda
{
    public int statusCode;
    public string headers;
    public string body;
    public bool isBase64Encoded;
}
[System.Serializable] 
public class body
{
    public string PlayerSessionId;
    public string PlayerId;
    public string GameSessionId;
    public string FleetId;
    public string FleetArn;
    public string CreationTime;
    public string Status;
    public string IpAddress;
    public string DnsName;
    public int Port;
}