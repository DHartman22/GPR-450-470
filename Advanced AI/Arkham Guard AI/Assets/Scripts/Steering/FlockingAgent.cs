using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//uses more traditional steering instead of just raycasts
public class FlockingAgent : MonoBehaviour
{
    public float mass;
    public float speed;
    public float maxSpeed;
    public float maxForce;
    public Vector2 velocity;
    public Vector2 direction;
    public float influenceDetectionRange;
    FlockingAgentManager manager;
    public Vector3 axis;
    public ObstacleAvoidanceSteering obstacleAvoidance;
    public bool isObstacleAvoiding = false; //Prevents other agents from using this agent's position for flocking purposes while obstacle avoiding
    [SerializeField]
    float turnSmoothTime = 0.1f;

    float turnSmoothVelocity;

    public Vector3 target;
    public float slowingRadius;
    public float health = 100;
    public Slider healthBar;

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
        Vector2 steerForce = Vector2.ClampMagnitude(steer, maxForce);
        Vector2 accel = steerForce / mass;
        if (Vector3.Distance(transform.position, target) < slowingRadius)
            velocity = Vector2.ClampMagnitude(velocity + accel, maxSpeed) * (Vector3.Distance(transform.position, target) * slowingRadius);
        else
            velocity = Vector2.ClampMagnitude(velocity + accel, maxSpeed);
        //velocity += velocity + steer;
        //velocity = velocity.normalized * maxSpeed;
        float targetAngle = Mathf.Atan2(velocity.normalized.y, velocity.normalized.x) * Mathf.Rad2Deg;

        targetAngle = Quaternion.AngleAxis(targetAngle, axis).eulerAngles.z;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.position = transform.position + (Vector3)velocity;
        //transform.rotation = Quaternion.AngleAxis(angle, axis);
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Bullet")
        {
            health -= collision.gameObject.GetComponent<Bullet>().damage;
            Destroy(collision.gameObject);
            if (health <= 0)
                maxSpeed = 0;
        }
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
        healthBar.value = health;
    }
}
