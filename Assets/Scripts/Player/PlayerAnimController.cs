using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerAnimController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetChild(2).gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        

    }
    [ClientRpc]
    public void StartAnimClientRpc(string str)
    {
        animator.SetBool(str, true);
    }
    [ClientRpc]
    public void StopAnimClientRpc(string str)
    {
        animator.SetBool(str, false);
    }
    [ServerRpc]
    public void StartAnimServerRpc()
    {
        StartAnimClientRpc("Running");
    }
    [ServerRpc]
    public void StopAnimServerRpc()
    {
        StopAnimClientRpc("Running");
    }
}