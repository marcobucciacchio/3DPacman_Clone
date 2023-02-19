using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ghost : MonoBehaviourPun
{
    public int pinkyReleaseTimer = 5;
    public int inkyReleaseTimer = 14;
    public int clydeReleaseTimer = 21;

    public float ghostReleaseTimer = 0;

    public bool isInGhostHouse = false;

    public float moveSpeed = 3.3f;

    public Node startingPosition;

    public Node homeNode;

    public int scatterModeTimer1 = 7;
    public int chaseModeTimer1 = 20;
    public int scatterModeTimer2 = 7;
    public int chaseModeTimer2 = 20;
    public int scatterModeTimer3 = 5;
    public int chaseModeTimer3 = 20;
    public int scatterModeTimer4 = 5;

    private int modeChangeIteration = 1;
    private float modeChangeTimer = 0;

    public RuntimeAnimatorController ghostUp;
    public RuntimeAnimatorController ghostDown;
    public RuntimeAnimatorController ghostLeft;
    public RuntimeAnimatorController ghostRight;
    public RuntimeAnimatorController idle;


    public enum Mode
    {
        Chase,
        Scatter,
        Frightened
    }


    Mode currentMode = Mode.Scatter;
    Mode previousMode;

    public enum GhostType
    {
        Red, 
        Pink,
        Blue,
        Orange
    }

    public GhostType ghostType = GhostType.Red;

    private GameObject[] pacMan;

    private Node currentNode, previousNode, targetNode;
    private Vector3 direction, nextDirection;


    private void Start()
    {

        pacMan = GameObject.FindGameObjectsWithTag("player");

        Node node = GetNodeAtPosition(transform.localPosition);
        if (node != null)
        {
            currentNode = node;
        }

        if (isInGhostHouse)
        {
            direction = Vector3.forward;
            targetNode = currentNode.neighbours[0];
        }
        else
        {
            direction = Vector3.right;
            targetNode = ChooseNextNode();
        }
      
        previousNode = currentNode;

        UpdateAnimatorController();
    }

    private void Update()
    {
        ModeUpdate();

        Move();

        ReleaseGhost();

        
    } 

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player")
        {
            GameObject.Find("Game").transform.GetComponent<GameBoard>().Restart();
        }

    }

    void UpdateAnimatorController()
    {
        if(direction == Vector3.right)
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = ghostRight;
        }else if(direction == Vector3.left)
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = ghostLeft;
        }
        else if (direction == Vector3.forward)
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = ghostUp;
        }
        else if (direction == Vector3.back)
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = ghostDown;
        }
        else
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = idle;
        }
    }

    void Move()
    {
        if(targetNode != currentNode && targetNode != null && !isInGhostHouse)
        {
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

                targetNode = ChooseNextNode();

                previousNode = currentNode;

                currentNode = null;

                UpdateAnimatorController();
            }
            else
            {
                transform.localPosition += direction * moveSpeed * Time.deltaTime;
            }
        }
    }

    void ModeUpdate()
    {
        if(currentMode != Mode.Frightened)
        {
            modeChangeTimer += Time.deltaTime;
            if(modeChangeIteration == 1)
            {
                if(currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer1)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
 
                }
                if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer1)
                {
                    modeChangeIteration = 2;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
          
                }
            }
            else if(modeChangeIteration == 2)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer2)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
    
                }
                if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer2)
                {
                    modeChangeIteration = 3;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;

                }
            }
            else if(modeChangeIteration == 3)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer3)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                 
                }
                if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer3)
                {
                    modeChangeIteration = 4;
                    ChangeMode(Mode.Scatter);

                }
            }
            else if(modeChangeIteration == 4)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer4)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
            }
        }
    }

    void ChangeMode(Mode m)
    {
        currentMode = m;
    }

    Vector3 GetBlueGhostTargetTile()
    {
        Vector3 tempinkyPos = GameObject.Find("Inky").transform.position;

        int inkyPositionX = Mathf.RoundToInt(tempinkyPos.x);
        int inkyPositionZ = Mathf.RoundToInt(tempinkyPos.z);
        tempinkyPos = new Vector3(inkyPositionX, 0f, inkyPositionZ);

    //    if (GetDistance(tempinkyPos, pacMan[0].transform.position) > GetDistance(tempinkyPos, pacMan[1].transform.position))
        {
            //select the position 2 tiles in front of pacman
            //draw vector from blinky to that position
            //double the length of that vec
            Vector3 pacmanPosition = pacMan[0].transform.position;
            Vector3 pacmanOrientation = pacMan[0].GetComponent<PacMan>().orientation;
            int pacmanPositionX = Mathf.RoundToInt(pacmanPosition.x);
            int pacmanPositionZ = Mathf.RoundToInt(pacmanPosition.z);

            Vector3 pacmanTile = new Vector3(pacmanPositionX, 0f, pacmanPositionZ);
            Vector3 targetTile = pacmanTile + (2 * pacmanOrientation);

            //temp vector for blinky pos
            Vector3 tempBlinkyPos = GameObject.Find("Blinky").transform.position;

            int blinkyPositionX = Mathf.RoundToInt(tempBlinkyPos.x);
            int blinkyPositionZ = Mathf.RoundToInt(tempBlinkyPos.z);

            tempBlinkyPos = new Vector3(blinkyPositionX, 0f, blinkyPositionZ);

            float distance = GetDistance(tempBlinkyPos, targetTile);
            distance *= 2;
            targetTile = new Vector3(tempBlinkyPos.x + distance, 0f, tempBlinkyPos.z + distance);

            return targetTile;
        }
/*        else
        {
            //select the position 2 tiles in front of pacman
            //draw vector from blinky to that position
            //double the length of that vec
            Vector3 pacmanPosition = pacMan[1].transform.position;
            Vector3 pacmanOrientation = pacMan[1].GetComponent<PacMan>().orientation;
            int pacmanPositionX = Mathf.RoundToInt(pacmanPosition.x);
            int pacmanPositionZ = Mathf.RoundToInt(pacmanPosition.z);

            Vector3 pacmanTile = new Vector3(pacmanPositionX, 0f, pacmanPositionZ);
            Vector3 targetTile = pacmanTile + (2 * pacmanOrientation);

            //temp vector for blinky pos
            Vector3 tempBlinkyPos = GameObject.Find("Blinky").transform.position;

            int blinkyPositionX = Mathf.RoundToInt(tempBlinkyPos.x);
            int blinkyPositionZ = Mathf.RoundToInt(tempBlinkyPos.z);

            tempBlinkyPos = new Vector3(blinkyPositionX, 0f, blinkyPositionZ);

            float distance = GetDistance(tempBlinkyPos, targetTile);
            distance *= 2;
            targetTile = new Vector3(tempBlinkyPos.x + distance, 0f, tempBlinkyPos.z + distance);

            return targetTile;
        }*/
    }

    Vector3 GetOrangeGhostTargetTile()
    {
        Vector3 tempclydePos = GameObject.Find("Clyde").transform.position;

        int clydePositionX = Mathf.RoundToInt(tempclydePos.x);
        int clydePositionZ = Mathf.RoundToInt(tempclydePos.z);
        tempclydePos = new Vector3(clydePositionX, 0f, clydePositionZ);

       // if (GetDistance(tempclydePos, pacMan[0].transform.position) > GetDistance(tempclydePos, pacMan[1].transform.position))
        {
            //calc distance from pacman
            //if distance is greater than 8 tiles the targeting is = to blinky
            //if less than 8 tiles, then his target is his homeNode

            Vector3 pacmanPosition = pacMan[0].transform.position;
            float distance = GetDistance(transform.localPosition, pacmanPosition);
            Vector3 targetTile = Vector3.zero;

            if (distance > 8)
            {
                targetTile = new Vector3(Mathf.RoundToInt(pacmanPosition.x), 0f, Mathf.RoundToInt(pacmanPosition.z));
            }
            else if (distance < 8)
            {
                targetTile = homeNode.transform.position;
            }

            return targetTile;
        }
/*        else
        {
            Vector3 pacmanPosition = pacMan[1].transform.position;
            float distance = GetDistance(transform.localPosition, pacmanPosition);
            Vector3 targetTile = Vector3.zero;

            if (distance > 8)
            {
                targetTile = new Vector3(Mathf.RoundToInt(pacmanPosition.x), 0f, Mathf.RoundToInt(pacmanPosition.z));
            }
            else if (distance < 8)
            {
                targetTile = homeNode.transform.position;
            }

            return targetTile;
        }*/
    }

    Vector3 GetRedGhostTargetTile()
    {
        Vector3 tempblinkyPos = GameObject.Find("Blinky").transform.position;

        int blinkyPositionX = Mathf.RoundToInt(tempblinkyPos.x);
        int blinkyPositionZ = Mathf.RoundToInt(tempblinkyPos.z);
        tempblinkyPos = new Vector3(blinkyPositionX, 0f, blinkyPositionZ);

//if (GetDistance(tempblinkyPos, pacMan[0].transform.position) > GetDistance(tempblinkyPos, pacMan[1].transform.position))
        {
            Vector3 pacmanPosition = pacMan[0].transform.position;
            Vector3 targetTile = new Vector3(Mathf.RoundToInt(pacmanPosition.x), 0f, Mathf.RoundToInt(pacmanPosition.z));
            return targetTile;
        }
 /*       else
        {
            Vector3 pacmanPosition = pacMan[1].transform.position;
            Vector3 targetTile = new Vector3(Mathf.RoundToInt(pacmanPosition.x), 0f, Mathf.RoundToInt(pacmanPosition.z));
            return targetTile;
        }*/
    }

    Vector3 GetPinkGhostTargetTile()
    {
        Vector3 temppinkyPos = GameObject.Find("Pinky").transform.position;

        int pinkyPositionX = Mathf.RoundToInt(temppinkyPos.x);
        int pinkyPositionZ = Mathf.RoundToInt(temppinkyPos.z);
        temppinkyPos = new Vector3(pinkyPositionX, 0f, pinkyPositionZ);

       // if (GetDistance(temppinkyPos, pacMan[0].transform.position) > GetDistance(temppinkyPos, pacMan[1].transform.position))
        {
            // four tiles ahead of pacman
            //take into account orientation
            Vector3 pacmanPosition = pacMan[0].transform.position;
            Vector3 pacmanOrientation = pacMan[0].GetComponent<PacMan>().orientation;
            int pacmanPositionX = Mathf.RoundToInt(pacmanPosition.x);
            int pacmanPositionZ = Mathf.RoundToInt(pacmanPosition.z);

            Vector3 pacmanTile = new Vector3(pacmanPositionX, 0f, pacmanPositionZ);
            Vector3 targetTile = pacmanTile + (4 * pacmanOrientation);

            return targetTile;
        }
/*        else
        {
            // four tiles ahead of pacman
            //take into account orientation
            Vector3 pacmanPosition = pacMan[1].transform.position;
            Vector3 pacmanOrientation = pacMan[1].GetComponent<PacMan>().orientation;
            int pacmanPositionX = Mathf.RoundToInt(pacmanPosition.x);
            int pacmanPositionZ = Mathf.RoundToInt(pacmanPosition.z);

            Vector3 pacmanTile = new Vector3(pacmanPositionX, 0f, pacmanPositionZ);
            Vector3 targetTile = pacmanTile + (4 * pacmanOrientation);

            return targetTile;
        }*/
    }

    Vector3 GetTargetTile()
    {
        Vector3 targetTile = Vector3.zero;

        if(ghostType == GhostType.Red)
        {
            targetTile = GetRedGhostTargetTile();

        }else if(ghostType == GhostType.Pink)
        {
            targetTile = GetPinkGhostTargetTile();
        }
        else if (ghostType == GhostType.Orange)
        {
            targetTile = GetOrangeGhostTargetTile();
        }
        else if (ghostType == GhostType.Blue)
        {
            targetTile = GetBlueGhostTargetTile();
        }
        return targetTile;
    }

    void ReleasePinkGhost()
    {
        if(ghostType == GhostType.Pink && isInGhostHouse)
        {
            isInGhostHouse = false;
        }
    }

    void ReleaseBlueGhost()
    {
        if(ghostType == GhostType.Blue)
        {
            isInGhostHouse = false;
        }
    }

    void ReleaseOrangeGhost()
    {
        if (ghostType == GhostType.Orange)
        {
            isInGhostHouse = false;
        }
    }

    void ReleaseGhost()
    {
        ghostReleaseTimer += Time.deltaTime;
        if(ghostReleaseTimer > pinkyReleaseTimer)
            ReleasePinkGhost();
        if (ghostReleaseTimer > inkyReleaseTimer)
            ReleaseBlueGhost();
        if (ghostReleaseTimer > clydeReleaseTimer)
            ReleaseOrangeGhost();

    }

    Node ChooseNextNode()
    {
        Vector3 targetTile = Vector3.zero;

        if(currentMode == Mode.Chase){

            targetTile = GetTargetTile();
        }else if(currentMode == Mode.Scatter)
        {
            targetTile = homeNode.transform.position;
        }

       // targetTile = GetTargetTile();

        Node moveToNode = null;

        Node[] foundNodes = new Node[4];
        Vector3[] foundNodesDirection = new Vector3[4];

        int nodeCounter = 0;

        for (int i = 0; i < currentNode.neighbours.Length; i++)
        {
            if(currentNode.validDirections[i] != direction * -1)
            {
                foundNodes[nodeCounter] = currentNode.neighbours[i];
                foundNodesDirection[nodeCounter] = currentNode.validDirections[i];
                nodeCounter++;
            }
        }
        if (foundNodes.Length == 1)
        {
            moveToNode = foundNodes[0];
            direction = foundNodesDirection[0];
        }
        if(foundNodes.Length > 1)
        {
            float leastDistance = 10000f;
            for (int i = 0; i < foundNodes.Length; i++)
            {
                if(foundNodesDirection[i] != Vector3.zero)
                {
                    float distance = GetDistance(foundNodes[i].transform.position, targetTile);
                    
                    if(distance < leastDistance)
                    {
                        leastDistance = distance;
                        moveToNode = foundNodes[i];
                        direction = foundNodesDirection[i];
                    }
                }
            }
        }
        return moveToNode;
    }

    Node GetNodeAtPosition(Vector3 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().nodePos[(int)pos.x, (int)pos.z];
       
        if(tile != null)
        {
            if(tile.GetComponent<Node>() != null)
            {
                return tile.GetComponent<Node>();
            }
        }
        return null;
    }

    GameObject GetPortal(Vector3 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().nodePos[(int)pos.x, (int)pos.z];
        if (tile != null)
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


    float LengthFromNode(Vector3 targetPosition)
    {
        Vector3 vec = targetPosition - previousNode.transform.position;
        return vec.sqrMagnitude;
    }

    bool OverShotTarget()
    {
        float nodeToTarget = LengthFromNode(targetNode.transform.position);
        float nodeToSelf = LengthFromNode(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }

    float GetDistance(Vector3 posA, Vector3 posB)
    {
        float dx = posA.x - posB.x;
        float dz = posA.z - posB.z;

        float distance = Mathf.Sqrt(dx * dx + dz * dz);
        return distance;
    }

}














