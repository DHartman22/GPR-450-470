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
    public bool isObstacleAvoiding = false; //Prevents other agents from using this agent's position for flocking purposes while obstacle avoiding
    [SerializeField]
    float turnSmoothTime = 0.1f;

    float turnSmoothVelocity;

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
        Vector2 steer = GetSteering();
        velocity += velocity + steer;
        velocity = velocity.normalized * maxSpeed;
        float targetAngle = Mathf.Atan2(velocity.normalized.y, velocity.normalized.x) * Mathf.Rad2Deg;

        targetAngle = Quaternion.AngleAxis(targetAngle, axis).eulerAngles.z;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.position = transform.position + (Vector3)velocity;
        transform.rotation = Quaternion.AngleAxis(angle, axis);
        
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
