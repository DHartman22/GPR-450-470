using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekFleeSteer : MonoBehaviour
{
    public Transform seekFleePoint;
    public float maxSpeed;
    public bool active;
    public enum SeekOrFlee
    {
        Seek,
        Flee
    }

    public SeekOrFlee mode;

    public Vector2 GetSteering(Vector2 position, Vector2 velocity)
    {
        if (!active)
            return Vector2.zero;

        if (mode == SeekOrFlee.Flee)
        {
            Vector2 desiredVel = (position - (Vector2)seekFleePoint.position).normalized * maxSpeed;
            return desiredVel - velocity;
        }
        else
        {
            Vector2 desiredVel = -(position - (Vector2)seekFleePoint.position).normalized * maxSpeed;
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
        if(Input.GetMouseButton(0))
        {
            active = true;
            seekFleePoint.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mode = SeekOrFlee.Seek;
        }
        else if (Input.GetMouseButton(1))
        {
            active = true;
            seekFleePoint.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mode = SeekOrFlee.Flee;
        }
        else
        {
            active = false;
        }
    }
}
