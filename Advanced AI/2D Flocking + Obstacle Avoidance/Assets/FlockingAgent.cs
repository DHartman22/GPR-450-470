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
    public SeekFleeSteer seekFlee;
    
    
    Vector2 GetSteering()
    {
        Vector2 totalSteering = Vector2.zero;
        totalSteering += seekFlee.GetSteering(transform.position, velocity);

        return totalSteering;
    }

    void MoveAgent()
    {
        Vector2 force = Vector2.ClampMagnitude(GetSteering(), maxForce);
        Vector2 accel = force / mass;
        velocity = Vector2.ClampMagnitude(velocity + accel, maxSpeed);
        transform.position = transform.position + (Vector3)velocity;
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
