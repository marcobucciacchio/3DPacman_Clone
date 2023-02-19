using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperPellet : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "player")
        {
            collider.GetComponent<PacMan>().EatSuperPellet();
            Destroy(gameObject);
        }
    }
}
