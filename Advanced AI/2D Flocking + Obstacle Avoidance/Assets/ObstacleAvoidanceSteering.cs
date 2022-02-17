using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidanceSteering : MonoBehaviour
{
    float gizmoRadius;

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
    public float stupidRayDistanceFromObstacle = 1f;
    public Vector3 up;
    
    public LineRenderer lineRenderer;

    public bool isSteering = false;


    public Vector2 GetSteering(Vector2 position, FlockingAgent agent, float radius)
    {
        agentDirection = (centerRay.transform.position - transform.position).normalized;
        Vector2[] rayDirs = { (centerRay.transform.position - transform.position).normalized ,
        (leftRay.transform.position - transform.position).normalized, (rightRay.transform.position - transform.position).normalized};
        //transform.position = transform.position + (Vector3)agentDirection.normalized * speed;
        if (obstacleDetected == false)
        {
            timeSpentTurning = 0;
            desiredDirection = Vector3.zero;
            return Vector2.zero;
        }
        //rgd.velocity = Vector2.zero;



        if (hitList[0].collider != null)
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
            //return contactNormal;
            //this solution is really dumb but it works
            //Draws a line outwards from the contact normal, stops, then sets the steering direction to be towards this point
            Ray stupidRay = new Ray(hitList[0].point, contactNormal);

            Debug.DrawLine(hitList[0].point, stupidRay.GetPoint(stupidRayDistanceFromObstacle), Color.green);
            float distanceModifier = maxDistance / Vector3.Distance(transform.position, hitList[0].point);

            return (stupidRay.GetPoint(stupidRayDistanceFromObstacle) - transform.position).normalized * distanceModifier;

        }
        else //One or both side rays hit something but not the center
        {
            // [0] = center, [1] = left, [2] = right
            //My lifetime xor usage has now increased to 1
            if (hitList[1].collider != null ^ hitList[2].collider != null) //returns true if only one of them is true but not both. 
            {
                if (hitList[1].collider != null)
                {
                    Vector2 contactNormal;
                    //do the contact normal stuff
                    if (hitList[1].collider.gameObject.layer == 7)
                    {
                        contactNormal = hitList[1].collider.gameObject.GetComponent<Plane>().normal;
                    }
                    else
                    {
                        contactNormal = (hitList[1].point - (Vector2)hitList[1].collider.transform.position).normalized;
                    }
                    Ray stupidRay = new Ray(hitList[1].point, contactNormal);

                    float distanceModifier = maxDistance/Vector3.Distance(transform.position, hitList[1].point);
                    Debug.DrawLine(hitList[1].point, stupidRay.GetPoint(stupidRayDistanceFromObstacle), Color.red);

                    return (stupidRay.GetPoint(stupidRayDistanceFromObstacle) - transform.position).normalized * distanceModifier;

                }
                if (hitList[2].collider != null)
                {
                    Vector2 contactNormal;
                    //do the contact normal stuff
                    if (hitList[2].collider.gameObject.layer == 7)
                    {
                        contactNormal = hitList[2].collider.gameObject.GetComponent<Plane>().normal;
                    }
                    else
                    {
                        contactNormal = (hitList[2].point - (Vector2)hitList[2].collider.transform.position).normalized;
                    }
                    Ray stupidRay = new Ray(hitList[2].point, contactNormal);
                    float distanceModifier = maxDistance / Vector3.Distance(transform.position, hitList[2].point);

                    Debug.DrawLine(hitList[2].point, stupidRay.GetPoint(stupidRayDistanceFromObstacle), Color.red);

                    return (stupidRay.GetPoint(stupidRayDistanceFromObstacle) - transform.position).normalized * distanceModifier;
                }
            }
            else if (hitList[1].collider != null && hitList[2].collider != null)
            {
                float ray1Distance = Vector2.Distance(hitList[1].point, transform.position);
                float ray2Distance = Vector2.Distance(hitList[2].point, transform.position);
                if (ray1Distance > ray2Distance)
                {
                    Vector2 contactNormal;
                    //do the contact normal stuff
                    if (hitList[2].collider.gameObject.layer == 7)
                    {
                        contactNormal = hitList[2].collider.gameObject.GetComponent<Plane>().normal;
                    }
                    else
                    {
                        contactNormal = (hitList[2].point - (Vector2)hitList[2].collider.transform.position).normalized;
                    }
                    Ray stupidRay = new Ray(hitList[2].point, contactNormal);

                    Debug.DrawLine(hitList[2].point, stupidRay.GetPoint(stupidRayDistanceFromObstacle), Color.red);
                    float distanceModifier = maxDistance / Vector3.Distance(transform.position, hitList[2].point);

                    return (stupidRay.GetPoint(stupidRayDistanceFromObstacle) - transform.position).normalized * distanceModifier;
                }
                else
                {
                    Vector2 contactNormal;
                    //do the contact normal stuff
                    if (hitList[1].collider.gameObject.layer == 7)
                    {
                        contactNormal = hitList[1].collider.gameObject.GetComponent<Plane>().normal;
                    }
                    else
                    {
                        contactNormal = (hitList[1].point - (Vector2)hitList[1].collider.transform.position).normalized;
                    }
                    Ray stupidRay = new Ray(hitList[1].point, contactNormal);
                    float distanceModifier = maxDistance/Vector3.Distance(transform.position, hitList[1].point);
                    Debug.DrawLine(hitList[1].point, stupidRay.GetPoint(stupidRayDistanceFromObstacle), Color.red);

                    return (stupidRay.GetPoint(stupidRayDistanceFromObstacle) - transform.position).normalized * distanceModifier;
                }
            }
        }
        return Vector2.zero;
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

        foreach (RaycastHit2D hit in hitList)
        {
            if (hit.collider != null)
            {
                obstacleDetected = true;
                break;
            }
            else
                obstacleDetected = false;
        }

    }

    void Start()
    {
        rays = new List<Ray>(3);
        hitList = new List<RaycastHit2D>(3);

        float leftRay = -angleDifferenceBetweenRays * Mathf.Deg2Rad;
        leftRayDir = new Vector2(agentDirection.x * Mathf.Cos(leftRay) - agentDirection.y * Mathf.Sin(leftRay),
            (agentDirection.x * Mathf.Sin(leftRay) - agentDirection.y * Mathf.Cos(leftRay)) * -1).normalized;


        float rightRay = angleDifferenceBetweenRays * Mathf.Deg2Rad;
        rightRayDir = new Vector2(agentDirection.x * Mathf.Cos(rightRay) - agentDirection.y * Mathf.Sin(rightRay),
            (agentDirection.x * Mathf.Sin(rightRay) - agentDirection.y * Mathf.Cos(rightRay)) * -1).normalized;


        //GameObject leftRayObj = Instantiate(new GameObject(), new Ray(transform.position, leftRayDir).GetPoint(maxDistance), Quaternion.identity, transform);
        //GameObject rightRayObj = Instantiate(new GameObject(), new Ray(transform.position, rightRayDir).GetPoint(maxDistance), Quaternion.identity, transform);
        //GameObject centerRayObj = Instantiate(new GameObject(), new Ray(transform.position, agentDirection).GetPoint(maxDistance), Quaternion.identity, transform);
    }

    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, rays[0].GetPoint(Vector3.Distance(transform.position, centerRay.transform.position)));
        Gizmos.DrawLine(transform.position, rays[1].GetPoint(Vector3.Distance(transform.position, leftRay.transform.position)));
        Gizmos.DrawLine(transform.position, rays[2].GetPoint(Vector3.Distance(transform.position, rightRay.transform.position)));

    }
}
