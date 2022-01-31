using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public Vector2 agentDirection;
    public Vector2 leftRayDir;
    public Vector2 rightRayDir;

    public Vector2 desiredDirection;
    public float maxDistance = 1;
    public Ray ray;
    public List<Ray> rays;
    RaycastHit hitInfo;
    public float speed = 1f;
    public LayerMask obstacleMask;
    public bool obstacleDetected;
    RaycastHit2D hitInfo2d;
    public float timeSpentTurning;
    public float maximumTurnTime;
    public float angleDifferenceBetweenRays;
    // Start is called before the first frame update
    void Start()
    {
        rays = new List<Ray>(3);

        float leftRay = angleDifferenceBetweenRays * Mathf.Deg2Rad;
        leftRayDir = new Vector2(agentDirection.x * Mathf.Cos(leftRay) - agentDirection.y * Mathf.Sin(leftRay),
            agentDirection.x * Mathf.Sin(leftRay) - agentDirection.y * Mathf.Cos(leftRay));

        float rightRay = -angleDifferenceBetweenRays * Mathf.Deg2Rad;
        rightRayDir = new Vector2(agentDirection.x * Mathf.Cos(rightRay) - agentDirection.y * Mathf.Sin(rightRay),
            agentDirection.x * Mathf.Sin(rightRay) - agentDirection.y * Mathf.Cos(rightRay));
    }

    void MoveAgent()
    {
        transform.position = transform.position + (Vector3)agentDirection.normalized * speed;
        if (obstacleDetected == false)
        {
            timeSpentTurning = 0;
            return;
        }
        Vector2 contactNormal = (hitInfo2d.point - (Vector2)hitInfo2d.collider.transform.position).normalized;
        float angleToChange = Vector2.Angle(agentDirection, contactNormal);
        Debug.Log(angleToChange);
        //transform.Rotate(new Vector3(0, 0, 1), angleToChange);
        float rads = angleToChange * Mathf.Deg2Rad;
        Vector2 newDirection = new Vector2(agentDirection.x * Mathf.Cos(rads) - agentDirection.y * Mathf.Sin(rads), 
            agentDirection.x * Mathf.Sin(rads) - agentDirection.y * Mathf.Cos(rads));
        desiredDirection = newDirection;
        RotateAgent();
    }

    void RotateAgent()
    {
        timeSpentTurning += Time.deltaTime;
        agentDirection = Vector2.Lerp(agentDirection, desiredDirection, timeSpentTurning/maximumTurnTime);

        float leftRay = angleDifferenceBetweenRays * Mathf.Deg2Rad;
        leftRayDir = new Vector2(agentDirection.x * Mathf.Cos(leftRay) - -agentDirection.y * Mathf.Sin(leftRay),
            agentDirection.x * Mathf.Sin(leftRay) - -agentDirection.y * Mathf.Cos(leftRay)).normalized;

        float rightRay = -angleDifferenceBetweenRays * Mathf.Deg2Rad;
        rightRayDir = new Vector2(agentDirection.x * Mathf.Cos(rightRay) - -agentDirection.y * Mathf.Sin(rightRay),
            agentDirection.x * Mathf.Sin(rightRay) - -agentDirection.y * Mathf.Cos(rightRay)).normalized;
        //flip y axis?
    }

    private void FixedUpdate()
    {
        rays.Clear();
        rays.Add(new Ray(transform.position, agentDirection));
        //rays[1] = 

        float leftRay = -angleDifferenceBetweenRays * Mathf.Deg2Rad;
        leftRayDir = new Vector2(agentDirection.x * Mathf.Cos(leftRay) - agentDirection.y * Mathf.Sin(leftRay),
            agentDirection.x * Mathf.Sin(leftRay) - agentDirection.y * Mathf.Cos(leftRay)).normalized;

        float rightRay = angleDifferenceBetweenRays * Mathf.Deg2Rad;
        rightRayDir = new Vector2(agentDirection.x * Mathf.Cos(rightRay) - agentDirection.y * Mathf.Sin(rightRay),
            agentDirection.x * Mathf.Sin(rightRay) - agentDirection.y * Mathf.Cos(rightRay)).normalized;

        rays.Add(new Ray(transform.position, leftRayDir));
        rays.Add(new Ray(transform.position, rightRayDir));

        hitInfo2d = Physics2D.Raycast(transform.position, agentDirection, maxDistance);
        if(hitInfo2d.collider != null)
        {
            obstacleDetected = true;
        }
        else
        {
            obstacleDetected = false;
        }
        MoveAgent();
    }

    private void OnDrawGizmos()
    {

        if (hitInfo2d.collider != null)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color= Color.red;
        }
        if(rays != null)
        {
            for(int i = 0; i < rays.Count; i++)
            {
                if(i == 0)
                {
                    if (hitInfo2d.collider != null)
                    {
                        Gizmos.color = Color.cyan;
                    }
                    else
                    {
                        Gizmos.color = Color.blue;
                    }
                }
                else
                {
                    if (hitInfo2d.collider != null)
                    {
                        Gizmos.color = Color.green;
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                    }
                }
                Gizmos.DrawLine(transform.position, rays[i].GetPoint(maxDistance));
            }
        }
    }
}
