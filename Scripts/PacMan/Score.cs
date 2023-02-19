using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Score : MonoBehaviourPun
{
    private float delayBeforeLoading = 5f;
    private float timeElapsed;
    int myScore = 0;
    int otherScore = 0;
    int totalPellets = 0;

    // Start is called before the first frame update
    void Start()
    {
       totalPellets = GameObject.Find("Pellets").transform.childCount;

    }

    void Update()
    {
        if(totalPellets != 0)
        {
            timeElapsed += Time.deltaTime;
        }
        else 
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene(0);
            }
        }

    }
    
    
    void OnGUI()
    {
       // if (base.photonView.IsMine)
        {
            GUI.Box(new Rect(10, 10, 120, 50), "Score");
            GUI.Box(new Rect(10, 30, 120, 30), myScore.ToString());

            GUI.Box(new Rect(10, 80, 120, 50), "Opp score");
            GUI.Box(new Rect(10, 100, 120, 30), otherScore.ToString());

            if (totalPellets != 0)
            {
                GUI.Box(new Rect(10, 150, 120, 50), "Pellets Left");
                GUI.Box(new Rect(10, 170, 120, 30), totalPellets.ToString());
            }
            else
            {
                string gameResult = "TIED GAME!";
                if (myScore > otherScore)
                {
                    gameResult = "YOU WIN!";
                }
                else if (myScore < otherScore)
                {
                    gameResult = "YOU LOSE!";
                }
                GUI.Box(new Rect(10, 150, 275, 50), "GAME OVER");
                GUI.Box(new Rect(10, 170, 275, 30), gameResult);
            }
        }
        
    }

    public void IncrementScore()
    {
        
        myScore++;
        totalPellets--;
    }

    public void IncrementOpponentScore()
    {
        otherScore++;
        totalPellets--;
    }
}
