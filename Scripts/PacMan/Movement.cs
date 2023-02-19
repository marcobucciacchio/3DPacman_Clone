using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviourPun
{

    [SerializeField]
    float Speed = 3.5f;
    [SerializeField]
    float boostTimer;
    bool boosting = false;
    [SerializeField]
    float respawnDuration = 3.0f;
    float respawnTimer;

    public Animator animator;
    private AudioSource wakaSound;
    private AudioSource powerUpSound;
    private AudioSource energize;
    private AudioSource deathSound;
    private Score score;
    private GameObject spawnPoint;
    //private Vector3 direction = Vector3.zero;
   

    private void Awake()
    {
        score = GameObject.FindGameObjectWithTag("Score").GetComponent<Score>();
        spawnPoint = GameObject.Find("SpawnPoint");
        AudioSource[] audio = GetComponents<AudioSource>();
        wakaSound = audio[2];
        powerUpSound = audio[1];
        deathSound = audio[0];
        energize = audio[3];
    }


    void FixedUpdate()
    {
        
        if (base.photonView.IsMine)
        {
            if ((Input.GetKey("d") || Input.GetKey("right")) && !Physics.Raycast(transform.position, Vector3.right, 0.65f, 1 << 9))
            {
                animator.SetBool("isUp", false);
                animator.SetBool("isDown", false);
                animator.SetBool("isLeft", false);
                transform.position += Vector3.right * Speed * Time.deltaTime;
                animator.SetBool("isRight", true);
            }
            else if ((Input.GetKey("a") || Input.GetKey("left")) && !Physics.Raycast(transform.position, -Vector3.right, 0.65f, 1 << 9))
            {
                animator.SetBool("isRight", false);
                animator.SetBool("isUp", false);
                animator.SetBool("isDown", false);
                transform.position += Vector3.left * Speed * Time.deltaTime;
                animator.SetBool("isLeft", true);
            }
            else if ((Input.GetKey("w") || Input.GetKey("up")) && !Physics.Raycast(transform.position, Vector3.forward, 0.65f, 1 << 9))
            {
                animator.SetBool("isRight", false);
                animator.SetBool("isLeft", false);
                animator.SetBool("isDown", false);
                transform.position += Vector3.forward * Speed * Time.deltaTime;
                animator.SetBool("isUp", true);
            }
            else if ((Input.GetKey("s") || Input.GetKey("down")) && !Physics.Raycast(transform.position, -Vector3.forward, 0.65f, 1 << 9))
            {
                animator.SetBool("isRight", false);
                animator.SetBool("isLeft", false);
                animator.SetBool("isUp", false);
                transform.position += Vector3.back * Speed * Time.deltaTime;
                animator.SetBool("isDown", true);
            }

            if (boosting)
            {
                boostTimer += Time.deltaTime;
                if (boostTimer >= 7)
                {
                    Speed = 3.5f;
                    boostTimer = 0;
                    boosting = false;
                }
                else
                {
                    Speed = 4.5f;
                }
            }
        }
    }



/*    private void OnCollisionEnter(Collision collision)
    {
        foreach(ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }

        if (collision.transform.tag == "PacGhost")
        {
            deathSound.Play();
            if (photonView.IsMine)
            {
                Respawn();
            }
        }
    }*/

    public void EatPellet()
    {
        if (base.photonView.IsMine)
        {
            score.IncrementScore();
            wakaSound.Play();
        }
        else
        {
            score.IncrementOpponentScore();
        }
       /* score.IncrementScore();
        wakaSound.Play();*/
    }

    public void EatSuperPellet()
    {
        if (base.photonView.IsMine)
        {
            score.IncrementScore();
            boosting = true;
            powerUpSound.Play();
            energize.Play();
            energize.SetScheduledEndTime(AudioSettings.dspTime + (7f));
        }
        else
        {
            score.IncrementOpponentScore();
        }
       /* score.IncrementScore();
        boosting = true;
        powerUpSound.Play();
        energize.Play();
        energize.SetScheduledEndTime(AudioSettings.dspTime + (7f));*/

    }


}



