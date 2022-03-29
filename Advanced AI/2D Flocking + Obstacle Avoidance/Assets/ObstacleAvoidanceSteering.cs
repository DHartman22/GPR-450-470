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
    public float newDirectionRayDistanceFromObstacle = 1f;
    public Vector3 up;
    
    public LineRenderer lineRenderer;

    public bool isSteering = false;


    public Vector2 GetSteering(Vector2 position, FlockingAgent agent, float radius)
    {
        agentDirection = (centerRay.transform.position - transform.position).normalized;
        Vector2[] rayDirs = { (centerRay.transform.position - transform.position).normalized ,
        (leftRay.transform.position - transform.position).normalized, (rightRay.transform.position - transform.position).normalized};

        float maxDistanceLeftRightRay = Vector3.Distance(transform.position, leftRay.transform.position); //same distance away
        //transform.position = transform.position + (Vector3)agentDirection.normalized * speed;
        if (obstacleDetected == false)
        {
            timeSpentTurning = 0;
            desiredDirection = Vector3.zero;
            agent.isObstacleAvoiding = false;
            return Vector2.zero;
        }
        agent.isObstacleAvoiding = true;
        //rgd.velocity = Vector2.zero;



        if (hitList[0].collider != null)
        {
            Vector2 contactNormal;
            //do the contact normal stuff
            if (hitList[0].collider.gameObject.layer == 7)
            {
                contactNormal = hitList[0].collider.gameObject.GetComponent<Plane>().normal + (hitList[0].point - (Vector2)transform.position).normalized;
            }
            else
            {
                contactNormal = (hitList[0].point - (Vector2)hitList[0].collider.transform.position).normalized;
            }

            if (contactNormal == Vector2.zero)
            {
                Debug.Log("Straight collision??");
                if (hitList[0].collider.gameObject.layer == 7)
                {
                    return hitList[0].collider.gameObject.GetComponent<Plane>().normal; 
                }
                else
                {
                    return (hitList[0].point - (Vector2)hitList[0].collider.transform.position).normalized;
                }
            }
            //return contactNormal;
            //this solution is really dumb but it works
            //Draws a line outwards from the contact normal, stops, then sets the steering direction to be towards this point
            Ray newDirectionRay = new Ray(hitList[0].point, contactNormal);

            Debug.DrawLine(hitList[0].point, newDirectionRay.GetPoint(newDirectionRayDistanceFromObstacle), Color.red);
            float distanceModifier = maxDistance / Vector3.Distance(transform.position, hitList[0].point);

            if (distanceModifier == float.PositiveInfinity)
            {
                //Stuck in wall solution
                if (hitList[0].collider.gameObject.layer == 7)
                {
                    return hitList[0].collider.gameObject.GetComponent<Plane>().normal;
                }
                else
                {
                    return (hitList[0].point - (Vector2)hitList[0].collider.transform.position).normalized;
                }
            }

            if (hitList[1].collider != null && hitList[2].collider != null) //Center ray is hitting plus both side rays
            {
                float ray1Distance = Vector2.Distance(hitList[1].point, transform.position);
                float ray2Distance = Vector2.Distance(hitList[2].point, transform.position);
                bool bothPlanes = hitList[1].collider.gameObject.GetComponent<Plane>() != null && hitList[2].collider.gameObject.GetComponent<Plane>() != null;
                if(bothPlanes)
                {
                    if(hitList[1].collider.gameObject.GetComponent<Plane>().normal != hitList[2].collider.gameObject.GetComponent<Plane>().normal)
                    {
                        transform.Rotate(0, 0, 180);
                        agent.velocity *= -1;
                        return agent.velocity.normalized;
                        //return (hitList[1].collider.gameObject.GetComponent<Plane>().normal + hitList[2].collider.gameObject.GetComponent<Plane>().normal).normalized;
                    }
                }
                Vector3 finalSteer = Vector3.zero;

                //do the contact normal stuff
                if (hitList[2].collider.gameObject.layer == 7)
                {
                    contactNormal = hitList[2].collider.gameObject.GetComponent<Plane>().normal + (hitList[2].point - (Vector2)transform.position).normalized;
                }
                else
                {
                    contactNormal = (hitList[2].point - (Vector2)hitList[2].collider.transform.position).normalized;
                }
                newDirectionRay = new Ray(hitList[2].point, contactNormal);

                Debug.DrawLine(hitList[2].point, newDirectionRay.GetPoint(newDirectionRayDistanceFromObstacle), Color.red);
                distanceModifier = maxDistanceLeftRightRay / Vector3.Distance(transform.position, hitList[2].point);

                Vector2 rightSteer = (newDirectionRay.GetPoint(newDirectionRayDistanceFromObstacle) - transform.position).normalized * distanceModifier;

                //do the contact normal stuff
                if (hitList[1].collider.gameObject.layer == 7)
                {
                    contactNormal = hitList[1].collider.gameObject.GetComponent<Plane>().normal + (hitList[1].point - (Vector2)transform.position).normalized;
                }
                else
                {
                    contactNormal = (hitList[1].point - (Vector2)hitList[1].collider.transform.position).normalized;
                }
                newDirectionRay = new Ray(hitList[1].point, contactNormal);
                distanceModifier = maxDistanceLeftRightRay / Vector3.Distance(transform.position, hitList[1].point);
                Debug.DrawLine(hitList[1].point, newDirectionRay.GetPoint(newDirectionRayDistanceFromObstacle), Color.red);

                Vector2 leftSteer = (newDirectionRay.GetPoint(newDirectionRayDistanceFromObstacle) - transform.position).normalized * distanceModifier;

                finalSteer = leftSteer + rightSteer;

                if (finalSteer.magnitude < 3 && bothPlanes)
                    finalSteer = rightSteer;

                Debug.Log(finalSteer.magnitude);

                finalSteer /= 2;

                
                return finalSteer; // average the two instead of picking just one to avoid shake

            }

            return (newDirectionRay.GetPoint(newDirectionRayDistanceFromObstacle) - transform.position).normalized;

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
                        contactNormal = hitList[1].collider.gameObject.GetComponent<Plane>().normal + (hitList[1].point - (Vector2)transform.position).normalized;
                    }
                    else
                    {
                        contactNormal = (hitList[1].point - (Vector2)hitList[1].collider.transform.position).normalized;
                    }
                    Ray newDirectionRay = new Ray(hitList[1].point, contactNormal);

                    float distanceModifier = maxDistanceLeftRightRay/Vector3.Distance(transform.position, hitList[1].point);
                    Debug.DrawLine(hitList[1].point, newDirectionRay.GetPoint(newDirectionRayDistanceFromObstacle), Color.red);

                    return (newDirectionRay.GetPoint(newDirectionRayDistanceFromObstacle) - transform.position).normalized;

                }
                if (hitList[2].collider != null)
                {
                    Vector2 contactNormal;
                    //do the contact normal stuff
                    if (hitList[2].collider.gameObject.layer == 7)
                    {
                        contactNormal = hitList[2].collider.gameObject.GetComponent<Plane>().normal + (hitList[2].point - (Vector2)transform.position).normalized;
                    }
                    else
                    {
                        contactNormal = (hitList[2].point - (Vector2)hitList[2].collider.transform.position).normalized;
                    }
                    Ray newDirectionRay = new Ray(hitList[2].point, contactNormal);
                    float distanceModifier = maxDistanceLeftRightRay / Vector3.Distance(transform.position, hitList[2].point);

                    Debug.DrawLine(hitList[2].point, newDirectionRay.GetPoint(newDirectionRayDistanceFromObstacle), Color.red);

                    return (newDirectionRay.GetPoint(newDirectionRayDistanceFromObstacle) - transform.position).normalized;
                }
            }
            else if (hitList[1].collider != null && hitList[2].collider != null)
            {
                float ray1Distance = Vector2.Distance(hitList[1].point, transform.position);
                float ray2Distance = Vector2.Distance(hitList[2].point, transform.position);
                Vector3 finalSteer = Vector3.zero;

                    Vector2 contactNormal;
                    //do the contact normal stuff
                    if (hitList[2].collider.gameObject.layer == 7)
                    {
                        contactNormal = hitList[2].collider.gameObject.GetComponent<Plane>().normal + (hitList[2].point - (Vector2)transform.position).normalized;
                    }
                    else
                    {
                        contactNormal = (hitList[2].point - (Vector2)hitList[2].collider.transform.position).normalized;
                    }
                    Ray newDirectionRay = new Ray(hitList[2].point, contactNormal);

                    Debug.DrawLine(hitList[2].point, newDirectionRay.GetPoint(newDirectionRayDistanceFromObstacle), Color.red);
                    float distanceModifier = maxDistanceLeftRightRay / Vector3.Distance(transform.position, hitList[2].point);

                    finalSteer += (newDirectionRay.GetPoint(newDirectionRayDistanceFromObstacle) - transform.position).normalized * distanceModifier;

                    //do the contact normal stuff
                    if (hitList[1].collider.gameObject.layer == 7)
                    {
                        contactNormal = hitList[1].collider.gameObject.GetComponent<Plane>().normal + (hitList[1].point - (Vector2)transform.position).normalized;
                    }
                    else
                    {
                        contactNormal = (hitList[1].point - (Vector2)hitList[1].collider.transform.position).normalized;
                    }
                    newDirectionRay = new Ray(hitList[1].point, contactNormal);
                    distanceModifier = maxDistanceLeftRightRay /Vector3.Distance(transform.position, hitList[1].point);
                    Debug.DrawLine(hitList[1].point, newDirectionRay.GetPoint(newDirectionRayDistanceFromObstacle), Color.red);

                finalSteer += (newDirectionRay.GetPoint(newDirectionRayDistanceFromObstacle) - transform.position).normalized * distanceModifier;

                finalSteer /= 2;
                return finalSteer; // average the two instead of picking just one to avoid shake

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
        hitList.Add(Physics2D.Raycast(transform.position, leftRayDir, Vector3.Distance(transform.position, leftRay.transform.position)));
        hitList.Add(Physics2D.Raycast(transform.position, rightRayDir, Vector3.Distance(transform.position, rightRay.transform.position)));
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
