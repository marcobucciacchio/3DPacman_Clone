using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviourPun
{

    private static int boardWidth = 28;
    private static int boardHeight = 31;

    public GameObject[,] nodePos = new GameObject[boardWidth, boardHeight];

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("povg_node");

        foreach (GameObject o in nodes)
        {
            Vector3 pos = o.transform.position;
            nodePos[(int)pos.x, (int)pos.z] = o;
        }

    }

    public void Restart()
    {

            GameObject pacMan = GameObject.FindGameObjectWithTag("player");
            pacMan.transform.GetComponent<PacMan>().Restart();
        
    }
}
