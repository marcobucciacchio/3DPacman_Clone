using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "player")
        {
            other.GetComponent<PacMan>().EatPellet();
            Destroy(gameObject);
        }
    }
}
