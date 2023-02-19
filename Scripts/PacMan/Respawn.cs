using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public Vector3 SpawnPoint;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PacMan")
        {
            other.transform.position = SpawnPoint;
        }
    }
}
