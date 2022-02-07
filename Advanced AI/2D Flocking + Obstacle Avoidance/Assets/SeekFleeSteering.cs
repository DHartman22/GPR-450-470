using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekFleeSteering : MonoBehaviour
{
    public Transform seekFleePoint;
    public float maxSpeed;

    public enum SeekOrFlee
    {
        Seek,
        Flee
    }

    public SeekOrFlee mode;

    public Vector2 GetSteering(Vector2 position, Vector2 velocity)
    {
        if(mode == SeekOrFlee.Flee)
        {
            Vector2 desiredVel = (transform.position - seekFleePoint.position).normalized * maxSpeed;
            return desiredVel - velocity;
        }
        else
        {
            Vector2 desiredVel = -(transform.position - seekFleePoint.position).normalized * maxSpeed;
            return desiredVel - velocity;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
