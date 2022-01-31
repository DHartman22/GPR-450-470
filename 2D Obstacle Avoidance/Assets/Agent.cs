using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public Vector2 agentDirection;
    public float maxDistance = 1;
    public Ray ray;
    RaycastHit hitInfo;
    public float speed = 1f;
    public LayerMask obstacleMask;
    public bool obstacleDetected;
    RaycastHit2D hitInfo2d;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void MoveAgent()
    {

        transform.position = transform.position + (Vector3)agentDirection.normalized * speed;
        if (obstacleDetected == false)
            return;
        Vector2 contactNormal = (hitInfo2d.point - (Vector2)hitInfo2d.collider.transform.position).normalized;
        float angleToChange = Vector2.Angle(agentDirection, contactNormal);
        Debug.Log(angleToChange);
        transform.Rotate(new Vector3(0, 0, 1), angleToChange);
        float rads = angleToChange * Mathf.Deg2Rad;
        Vector2 newDirection = new Vector2(agentDirection.x * Mathf.Cos(rads) - agentDirection.y * Mathf.Sin(rads), 
            agentDirection.x * Mathf.Sin(rads) - agentDirection.y * Mathf.Cos(rads));
        agentDirection = newDirection;
    }

    private void FixedUpdate()
    {
        ray = new Ray(transform.position, agentDirection); 
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
        Gizmos.DrawLine(transform.position, ray.GetPoint(maxDistance));
    }
}
