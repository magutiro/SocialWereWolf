using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpwnPoint : MonoBehaviour
{
    Transform[] _spwnPoints;
    // Start is called before the first frame update
    void Awake()
    {
        _spwnPoints = GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnPlayer(GameObject player, int id)
    {
        player.transform.position = _spwnPoints[id].position;
    }
}
