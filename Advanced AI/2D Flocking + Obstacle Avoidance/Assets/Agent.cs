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
    public float correctionSpeed = 1f;
    
    public LayerMask obstacleMask;
    public LayerMask planeMask;

    public bool obstacleDetected;
    RaycastHit2D hitInfo2d;
    List<RaycastHit2D> hitList;
    public float timeSpentTurning;
    public float turningSpeed;
    public float angleDifferenceBetweenRays;

    public GameObject centerRay;
    public GameObject leftRay;
    public GameObject rightRay;

    public Vector3 up;
    public Rigidbody2D rgd;

    public LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        rays = new List<Ray>(3);
        hitList = new List<RaycastHit2D>(3);

        float leftRay = -angleDifferenceBetweenRays * Mathf.Deg2Rad;
        leftRayDir = new Vector2(agentDirection.x * Mathf.Cos(leftRay) - agentDirection.y * Mathf.Sin(leftRay),
            (agentDirection.x * Mathf.Sin(leftRay) - agentDirection.y * Mathf.Cos(leftRay))*-1).normalized;


        float rightRay = angleDifferenceBetweenRays * Mathf.Deg2Rad;
        rightRayDir = new Vector2(agentDirection.x * Mathf.Cos(rightRay) - agentDirection.y * Mathf.Sin(rightRay),
            (agentDirection.x * Mathf.Sin(rightRay) - agentDirection.y * Mathf.Cos(rightRay))*-1).normalized;


        //GameObject leftRayObj = Instantiate(new GameObject(), new Ray(transform.position, leftRayDir).GetPoint(maxDistance), Quaternion.identity, transform);
        //GameObject rightRayObj = Instantiate(new GameObject(), new Ray(transform.position, rightRayDir).GetPoint(maxDistance), Quaternion.identity, transform);
        //GameObject centerRayObj = Instantiate(new GameObject(), new Ray(transform.position, agentDirection).GetPoint(maxDistance), Quaternion.identity, transform);
    }

    void MoveAgent()
    {
        agentDirection = (centerRay.transform.position - transform.position).normalized;
        Vector2[] rayDirs = { (centerRay.transform.position - transform.position).normalized ,
        (leftRay.transform.position - transform.position).normalized, (rightRay.transform.position - transform.position).normalized};
        //transform.position = transform.position + (Vector3)agentDirection.normalized * speed;
        rgd.velocity = agentDirection * speed;
        if (obstacleDetected == false)
        {
            timeSpentTurning = 0;
            desiredDirection = Vector3.zero;
            return;
        }
        //rgd.velocity = Vector2.zero;

        
        
        if(hitList[0].collider != null)
        {
            Vector2 contactNormal;
            //do the contact normal stuff
            if (hitList[0].collider.gameObject.layer == 7)
            {
                contactNormal = hitList[0].collider.gameObject.GetComponent<Plane>().normal;
            }
            else
            {
                contactNormal = (hitList[0].point - (Vector2)hitList[0].collider.transform.position).normalized;
            }
            //this solution is really dumb but it works
            Ray stupidRay = new Ray(hitList[0].point, contactNormal);
            if(Vector3.Distance(stupidRay.GetPoint(1), leftRay.transform.position) < Vector3.Distance(stupidRay.GetPoint(1), rightRay.transform.position))
            {
                transform.Rotate(new Vector3(0, 0, 1), turningSpeed);
            }
            else
            {
                transform.Rotate(new Vector3(0, 0, -1), turningSpeed);
            }
        }
        else
        {
            //xor!
            if (hitList[1].collider != null ^ hitList[2].collider != null) //returns true if only one of them is true but not both. If, somehow, these two get active and not 0 then no action is needed
            {
                if (hitList[1].collider != null)
                {
                    transform.Rotate(new Vector3(0, 0, -1), turningSpeed);
                }
                if (hitList[2].collider != null)
                {
                    transform.Rotate(new Vector3(0, 0, 1), turningSpeed);
                }
            }
            else if (hitList[1].collider != null && hitList[2].collider != null)
            {
                float ray1Distance = Vector2.Distance(hitList[1].point, transform.position);
                float ray2Distance = Vector2.Distance(hitList[2].point, transform.position);
                if (ray1Distance > ray2Distance)
                {
                    transform.Rotate(new Vector3(0, 0, 1), turningSpeed);
                }
                else
                {
                    transform.Rotate(new Vector3(0, 0, -1), turningSpeed);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        rays.Clear();
        hitList.Clear();
        rays.Add(new Ray(transform.position, agentDirection));
        //rays[1] = 

        float leftRayRads = -angleDifferenceBetweenRays * Mathf.Deg2Rad;
        leftRayDir = new Vector2(agentDirection.x * Mathf.Cos(leftRayRads) - agentDirection.y * Mathf.Sin(leftRayRads),
            agentDirection.x * Mathf.Sin(leftRayRads) - agentDirection.y * Mathf.Cos(leftRayRads)).normalized;

        float rightRayRads = angleDifferenceBetweenRays * Mathf.Deg2Rad;
        rightRayDir = new Vector2(agentDirection.x * Mathf.Cos(rightRayRads) - agentDirection.y * Mathf.Sin(rightRayRads),
            agentDirection.x * Mathf.Sin(rightRayRads) - agentDirection.y * Mathf.Cos(rightRayRads)).normalized;

        leftRayDir = (leftRay.transform.position - transform.position).normalized;
        rightRayDir = (rightRay.transform.position - transform.position).normalized;

        rays.Add(new Ray(transform.position, leftRayDir));
        rays.Add(new Ray(transform.position, rightRayDir));

        hitList.Add(Physics2D.Raycast(transform.position, agentDirection, maxDistance));
        hitList.Add(Physics2D.Raycast(transform.position, leftRayDir, maxDistance));
        hitList.Add(Physics2D.Raycast(transform.position, rightRayDir, maxDistance));

        foreach(RaycastHit2D hit in hitList)
        {
            if(hit.collider != null)
            {
                obstacleDetected = true;
                break;
            }
            else
                obstacleDetected = false;
        }
        MoveAgent();
    }

    void UpdateLines()
    {

    }

    private void OnDrawGizmos()
    {

        if (hitList[0].collider != null)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color= Color.red;
        }
                Gizmos.DrawLine(transform.position, rays[0].GetPoint(maxDistance));
    }
}
