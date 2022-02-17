using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//uses more traditional steering instead of just raycasts
public class FlockingAgent : MonoBehaviour
{
    public float mass;
    public float speed;
    public float maxSpeed;
    public float maxForce;
    public Vector2 velocity;
    public Vector2 direction;
    FlockingAgentManager manager;
    public Vector3 axis;
    public ObstacleAvoidanceSteering obstacleAvoidance;
    
    Vector2 GetSteering()
    {
        return manager.GetSteering(this);
    }

    public Vector2 GetObstacleAvoidanceSteering()
    {
        return obstacleAvoidance.GetSteering(transform.position, this, -1f);
    }

    void MoveAgent()
    {

        velocity += velocity + GetSteering();
        velocity = velocity.normalized * maxSpeed;
        //transform.Rotate(new Vector3(0, 0, 1), Vector3.Angle(transform.position.normalized, (Vector3)velocity.normalized));
        float angle = Mathf.Atan2(velocity.normalized.y, velocity.normalized.x) * Mathf.Rad2Deg;
        transform.position = transform.position + (Vector3)velocity;
        transform.rotation = Quaternion.AngleAxis(angle, axis);
        
        //transform.LookAt(transform.position + (Vector3)velocity);
    }

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindObjectOfType<FlockingAgentManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveAgent();
    }
}
