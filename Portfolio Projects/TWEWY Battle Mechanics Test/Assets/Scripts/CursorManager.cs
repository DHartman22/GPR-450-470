using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Decides what action will be executed based off the cursor's movement. 
public class CursorManager : MonoBehaviour
{
    public List<Vector2> lastMousePositions = new List<Vector2>();
    public int positionsToTrack = 5;
    public float mouseVelocity = 0f;
    public int positionFrame = 0;
    public LineRenderer lineRenderer;
    public bool moving = false;
    public Vector2 initialPress;
    public float velocity;
    public float deviation;
    public float timeBetweenPositionChecks = 16.67f;
    public float timeSinceLastPositionCheck = 0;
    public FollowMouseMovement playerMovement;

    public Color circleTrailColor;
    public Color defaultColor;

    public LayerMask enemy;
    public LayerMask player;

    public float timeForMouseDrag;
    public float maxTimeForTap;

    public enum AttackType
    {
        Undecided,
        Moving,
        ScratchEmptySpace,
        TapEmptySpace,
        DragEmptySpace,
        MovePlayer,
        SlashEnemy
    }

    public AttackType attack;

    [SerializeField]
    float totalLineDistance;

    [SerializeField]
    float slashDeviationLimit;
    [SerializeField]
    float slashVelocityRequirement;

    [SerializeField]
    float dragEmptySpaceDeviationRequirement;

    [SerializeField]
    float timeSinceInitialTouch;

    [SerializeField]
    float timeRequiredForDrag;

    [SerializeField]
    Vector2 boxcastSize;

    Ray2D rayToDraw;

    // Start is called before the first frame update
    void Start()
    {
        lastMousePositions = new List<Vector2>();
    }

    private void Update()
    {
        InputLoop();
    }

    void InputLoop()
    {
        if (Input.GetMouseButtonDown(0))
        {
            timeSinceInitialTouch = 0;
            attack = AttackType.Undecided;
            lineRenderer.positionCount = 0;
            lastMousePositions.Clear();
            initialPress = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            timeSinceInitialTouch += Time.deltaTime;
            timeSinceLastPositionCheck += Time.deltaTime;
            if (timeSinceLastPositionCheck >= timeBetweenPositionChecks)
            {
                timeSinceLastPositionCheck -= timeBetweenPositionChecks; //Carries leftover time over to next cycle
                ProcessInput();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (lineRenderer.positionCount < positionsToTrack)
            {
                TapEmptySpaceCheck(lastMousePositions[0]);
                return;
            }
            if(playerMovement.moving)
            {
                playerMovement.EndMove();
                
            }
            else
            {
                SlashCheck();
                TapEmptySpaceCheck(lastMousePositions[0]);
                playerMovement.EndDrag();
                playerMovement.EndScratch();
            }
            lineRenderer.positionCount = 0;
            lastMousePositions.Clear();
        }
        else
        {
            lineRenderer.positionCount = 0;
            lastMousePositions.Clear();
        }
    }

    void ProcessInput()
    {
        RecordPosition();
        CalculateVelocityDeviation();
        DrawCursorTrail();
        //SlashCheck();
        PlayerDragCheck();
        if(attack == AttackType.Undecided)
        {
            EmptySpaceScratchCheck();
            DragEmptySpaceCheck();
        }
        if(attack == AttackType.DragEmptySpace)
        {
            DragEmptySpaceCheck();

        }
        if (attack == AttackType.ScratchEmptySpace)
        {
            EmptySpaceScratchCheck();

        }
    }

    void TapEmptySpaceCheck(Vector2 target)
    {
        if (attack != AttackType.Undecided)
            return;

        if(maxTimeForTap >= timeSinceInitialTouch)
        {
            Debug.Log("TapEmptySpace");
            attack = AttackType.TapEmptySpace;
            playerMovement.Shoot(target);
        }
    }

    private void DragEmptySpaceCheck()
    {
        if(timeSinceInitialTouch >= timeRequiredForDrag)
        {
            attack = AttackType.DragEmptySpace;
            playerMovement.DragFire(GetLatestPosition());
            Debug.Log("DragEmptySpace");
        }
    }

    private void EmptySpaceScratchCheck()
    {
        if(deviation >= dragEmptySpaceDeviationRequirement && (attack == AttackType.Undecided || attack == AttackType.ScratchEmptySpace) )
        {
            //
            attack = AttackType.ScratchEmptySpace;
            playerMovement.Scratch(GetLatestPosition());
            Debug.Log("ScratchEmptySpace");
            
        }
    }

    private void PlayerDragCheck()
    {
        if(Vector2.Distance(initialPress, playerMovement.gameObject.transform.position) < playerMovement.playerDragRadius || playerMovement.moving)
        {
            playerMovement.Move(lastMousePositions[0]);
            attack = AttackType.MovePlayer;
        }
    }

    private void CalculateVelocityDeviation()
    {
        if (lineRenderer.positionCount == lastMousePositions.Count) //prevents check from occuring before 5 mouse positions are recorded
        {
            totalLineDistance = 0f;
            for (int i = 0; i < lastMousePositions.Count - 1; i++) //stop on the second to last to avoid invalid index
            {
                totalLineDistance += Vector2.Distance(lastMousePositions[i], lastMousePositions[i + 1]);
            }
            deviation = Mathf.Abs(totalLineDistance - Vector2.Distance(lastMousePositions[0], lastMousePositions[lastMousePositions.Count - 1]));

            velocity = totalLineDistance / (positionsToTrack); //
        }
    }

    

    private void SlashCheck()
    {
        if (attack != AttackType.Undecided)
            return;

        if(lineRenderer.positionCount == lastMousePositions.Count) //prevents check from occuring before 5 mouse positions are recorded
        {
            //Take the first and last recorded positions of the line 
            //Record how much the other positions deviate from the line
            //If the deviation is low and line distance is long enough, return true
            //Only run this function on releasing cursor

            //Idea for deviation
            //Record the distance between each recorded point
            //Add up the recorded distances between each point, then compare it to the distance between the first and last point
            //The closer the two values are, the less the deviation
            //This is really smart and simple good job
            
            if(deviation <= slashDeviationLimit && velocity >= slashVelocityRequirement)
            {
                Debug.Log("Success! Velocity = " + velocity + ", deviation = " + deviation);

                //Raycast to determine what is being slashed

                    RaycastHit2D hit = Physics2D.Raycast(lastMousePositions[lastMousePositions.Count - 1], (lastMousePositions[0] - lastMousePositions[lastMousePositions.Count - 1]).normalized,
                        Vector2.Distance(lastMousePositions[lastMousePositions.Count - 1], lastMousePositions[0]));
                    //Debug.DrawRay(GetLatestPosition(), hit.point, Color.yellow);
                    //RaycastHit2D hit = Physics2D.cast(lastMousePositions[(lastMousePositions.Count - 1)/2], boxcastSize, 0, );
                    if (hit.collider != null)
                    {
                        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                        {
                            Debug.Log("Slash Enemy");
                            playerMovement.StartSlash(hit.collider.transform.position);
                            attack = AttackType.SlashEnemy;
                            return;
                        }
                        else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                        {
                            Debug.Log("Slash Player");
                        }
                    }
                    else
                    {
                        Debug.Log("Slash Empty Space");
                    }
                
                
            }
            else
            {
                Debug.LogError("Failed, velocity = " + velocity + ", deviation = " + deviation);
            }
        }
        else
        {
            Debug.LogError("Not enough positions");
        }
    }

    void DrawCursorTrail()
    {
        for(int i = 0; i < lastMousePositions.Count; i++)
        {
            if (lineRenderer.positionCount < lastMousePositions.Count)
                lineRenderer.positionCount++;

            lineRenderer.SetPosition(i, lastMousePositions[i]);
        }
    }

    void RecordPosition()
    {
        lastMousePositions.Insert(0, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        while(lastMousePositions.Count > positionsToTrack)
        {
            lastMousePositions.RemoveAt(lastMousePositions.Count - 1);
        }
    }

    public Vector2 GetLatestPosition()
    {
        return lastMousePositions[0];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        //Gizmos.DrawLine(lastMousePositions[0], lastMousePositions[lastMousePositions.Count-1]);
    }
}
