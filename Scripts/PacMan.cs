using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PacMan : MonoBehaviourPun
{

    public static GameObject LocalPlayerInstance;

    public AudioClip chomp1;
    public AudioClip chomp2;
    public AudioClip death;

    public float Speed = 6f;
    public float boostTimer;
    bool boosting = false;
    public float respawnDuration = 3.0f;

    private bool playedChomp1 = false;

    private AudioSource audio;

    public Vector3 orientation;

    private Node currentNode ,previousNode, targetNode;

    private Node startingPosition;

    public Animator animator;
    private Score score;
    private Vector3 direction = Vector3.zero;
    private Vector3 nextDirection;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }
        DontDestroyOnLoad(gameObject);

    }
    private void Start() 
    {
            audio = transform.GetComponent<AudioSource>();
            animator = GetComponent<Animator>();
            Node node = GetNodeAtPosition(transform.localPosition);

            startingPosition = node;
            if (node != null)
            {
                currentNode = node;
            }
            direction = Vector3.zero;
            orientation = Vector3.zero;
            ChangePosition(direction);

            score = GameObject.FindGameObjectWithTag("Score").GetComponent<Score>();
    }
    /*
        void OnCollisionEnter(Collision collision)
        {
            if (photonView.IsMine)
            {
                if(collision.collider.tag == "player")
                {
                    Physics.IgnoreCollision(this.GetComponent<Collider>(), oth, true);
                }
            }
        }*/

    void OnTriggerEnter(Collider other)
    {

            if (other.gameObject.tag == "player")
            {
                Physics.IgnoreCollision(this.GetComponent<Collider>(), other, true);
            }
        
    }



    public void Restart()
    {
        if (photonView.IsMine)
        {
            transform.position = startingPosition.transform.localPosition;

            currentNode = startingPosition;

            ChangePosition(direction);

            audio.PlayOneShot(death);
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            CheckInput();
            Move();
            UpdateOrientation();
            UpdateAnimationState();

            if (boosting)
            {
                boostTimer += Time.deltaTime;
                if (boostTimer >= 7)
                {
                    Speed = 8f;
                    boostTimer = 0;
                    boosting = false;
                }
                else
                {
                    Speed = 6f;
                }
            }
        }
    }

    void PlayChompSound()
    {
        if (playedChomp1)
        {
            //play chomp2, set playchomp1 = false;
            audio.PlayOneShot(chomp2);
            playedChomp1 = false;
        }
        else
        {
            //play chom1, set playchomp1 = true;
            audio.PlayOneShot(chomp1);
            playedChomp1 = true;
        }
    }

    void UpdateAnimationState()
    {
        if(direction == Vector3.zero)
        {
            GetComponent<Animator>().enabled = false;
        }
        else
        {
            GetComponent<Animator>().enabled = true;
        }
    }

    Node GetNodeAtPosition(Vector3 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().nodePos[(int)pos.x, (int)pos.z];
        if(tile != null)
        {
            return tile.GetComponent<Node>();
        }
        return null;
    }


    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            ChangePosition(Vector3.left);

        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            ChangePosition(Vector3.right);

        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            ChangePosition(Vector3.forward);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)|| Input.GetKeyDown(KeyCode.S))
        {
            ChangePosition(Vector3.back);
        }
        
    }

    void ChangePosition(Vector3 d)
    {
        if (d != direction)
            nextDirection = d;

        if(currentNode != null)
        {
            Node moveToNode = CanMove(d);
            if(moveToNode != null)
            {
                direction = d;
                targetNode = moveToNode;
                previousNode = currentNode;
                currentNode = null;
            }
        }
    }

    void Move()
    {
        if(targetNode != currentNode && targetNode != null)
        {
            if(nextDirection == direction * -1)
            {
                direction *= -1;
                Node temp = targetNode;
                targetNode = previousNode;
                previousNode = temp;
            }

            if (OverShotTarget())
            {
                currentNode = targetNode;

                transform.localPosition = currentNode.transform.position;

                GameObject otherPortal = GetPortal(currentNode.transform.position);

                if(otherPortal != null)
                {
                    transform.localPosition = otherPortal.transform.position;
                    currentNode = otherPortal.GetComponent<Node>();
                }

                Node moveToNode = CanMove(nextDirection);
                
                if (moveToNode != null)
                    direction = nextDirection;
                
                if (moveToNode == null)
                    moveToNode = CanMove(direction);
                
                if (moveToNode != null)
                {
                    targetNode = moveToNode;
                    previousNode = currentNode;
                    currentNode = null;
                }else{
                    direction = Vector3.zero;
                }
            }else{
                transform.localPosition += (direction * Speed) * Time.deltaTime;
            }

        }
       
    }
    

    void MoveToNode(Vector3 d)
    {
        Node moveToNode = CanMove(d);
        if(moveToNode != null)
        {
            transform.localPosition = moveToNode.transform.position;
            currentNode = moveToNode;
        }
    }

    void UpdateOrientation()
    {
       //if (base.photonView.IsMine)
        {
            if (direction == Vector3.left)
            {
                orientation = Vector3.left;
                transform.localScale = new Vector3(.75f, .75f, .75f);
                transform.localRotation = Quaternion.Euler(180, 90, 90);
            }
            else if (direction == Vector3.right)
            {
                orientation = Vector3.right;
                transform.localScale = new Vector3(.75f, .75f, .75f);
                transform.localRotation = Quaternion.Euler(0, 90, 90);
            }
            else if (direction == Vector3.forward)
            {
                orientation = Vector3.forward;
                transform.localScale = new Vector3(.75f, .75f, .75f);
                transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
            else if (direction == Vector3.back)
            {
                orientation = Vector3.back;
                transform.localScale = new Vector3(.75f, .75f, .75f);
                transform.localRotation = Quaternion.Euler(0, 180, 90);
            }
        }

    }

    Node CanMove(Vector3 d)
    {
        Node moveToNode = null;
        for (int i = 0; i < currentNode.neighbours.Length; i++)
        {
            if(currentNode.validDirections[i] == d)
            {
                moveToNode = currentNode.neighbours[i];
                break;
            }
        }
        return moveToNode;
    }


    public void EatPellet()
    {
        if (base.photonView.IsMine)
        {
            score.IncrementScore();
            PlayChompSound();
        }
        else
        {
            score.IncrementOpponentScore();
        }

    }

    public void EatSuperPellet()
    {
        if (base.photonView.IsMine)
        {
            score.IncrementScore();
            boosting = true;
            PlayChompSound();
        }
        else
        {
            score.IncrementOpponentScore();
        }
    }

    bool OverShotTarget()
    {
        float nodeToTarget = LengthFromNode(targetNode.transform.position);
        float nodeToSelf = LengthFromNode(transform.position);

        return nodeToSelf > nodeToTarget;
    }

    float LengthFromNode(Vector3 targetPosition)
    {
        Vector3 vec = targetPosition - previousNode.transform.position;
        return vec.sqrMagnitude;
    }

    GameObject GetPortal(Vector3 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().nodePos[(int)pos.x, (int)pos.z];
        if(tile != null)
        {
            if (tile.GetComponent<Tile>() != null)
            {
                if (tile.GetComponent<Tile>().isPortal)
                {
                    GameObject otherPortal = tile.GetComponent<Tile>().portalReciever;
                    return otherPortal;
                }
            }
        }
        return null;
    }

  
}
