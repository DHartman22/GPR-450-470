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
    public float seekFleeWeight;
    public float alignWeight;
    public float cohesionWeight;
    public float separationWeight;
    public SeekFleeSteer seekFlee;
    public SeparationSteer separation;
    public CohesionSteer cohesion;
    
    Vector2 GetSteering()
    {
        Vector2 totalSteering = Vector2.zero;
        totalSteering += seekFlee.GetSteering(transform.position, velocity);
        totalSteering += separation.GetSteering(transform.position, velocity) * separationWeight;
        totalSteering += cohesion.GetSteering(transform.position, velocity) * cohesionWeight;
        return totalSteering;
    }

    void MoveAgent()
    {
        Vector2 force = Vector2.ClampMagnitude(GetSteering(), maxForce);
        Vector2 accel = force / mass;
        velocity = Vector2.ClampMagnitude(velocity + accel, maxSpeed);
        //transform.Rotate(new Vector3(0, 0, 1), Vector3.Angle(transform.position.normalized, (Vector3)velocity.normalized));
        float angle = Mathf.Atan2(velocity.normalized.x, velocity.normalized.y) * Mathf.Rad2Deg + transform.eulerAngles.z;
        transform.position = transform.position + (Vector3)velocity;
        //transform.rotation = Quaternion.Euler(0f, 0f, angle);
        //transform.LookAt(transform.position + (Vector3)velocity);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveAgent();
    }
}
