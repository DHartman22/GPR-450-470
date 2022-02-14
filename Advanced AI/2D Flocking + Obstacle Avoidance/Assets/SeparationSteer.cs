using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeparationSteer : MonoBehaviour
{
    public float separationDistance;
    public Vector2 GetSteering(Vector2 position, Vector2 velocity)
    {
        List<FlockingAgent> boids = new List<FlockingAgent>();
        boids.AddRange(GameObject.FindObjectsOfType<FlockingAgent>());
        Vector2 pointToSeparateFrom = Vector2.zero;
        foreach(FlockingAgent b in boids)
        {
            if(Vector2.Distance(b.transform.position, position) < separationDistance)
            {
                pointToSeparateFrom += ((position - (Vector2)b.transform.position).normalized);
            }
        }
        
        return pointToSeparateFrom.normalized;
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
