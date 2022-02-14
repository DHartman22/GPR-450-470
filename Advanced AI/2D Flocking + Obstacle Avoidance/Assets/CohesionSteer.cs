using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CohesionSteer : MonoBehaviour
{

    public float minDistanceToPoint;
    public Vector2 GetSteering(Vector2 position, Vector2 velocity)
    {
        List<FlockingAgent> boids = new List<FlockingAgent>();
        boids.AddRange(GameObject.FindObjectsOfType<FlockingAgent>());
        Vector2 pointToSeparateFrom = Vector2.zero;
        Vector2 avgPos = Vector2.zero;
        foreach (FlockingAgent b in boids)
        {
            pointToSeparateFrom += ((position - (Vector2)b.transform.position).normalized);
            avgPos += (Vector2)b.transform.position;
        }
        avgPos /= boids.Count;
        
        if(Vector2.Distance(position, avgPos) > minDistanceToPoint)
        {
            return (avgPos - position).normalized;

        }
        else
        {
            return Vector2.zero;
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
