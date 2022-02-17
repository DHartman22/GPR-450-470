using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeparationSteer : MonoBehaviour
{
    float gizmoRadius;
    public Vector2 GetSteering(Vector2 position, FlockingAgent agent, float radius)
    {
        gizmoRadius = radius;
        List<FlockingAgent> boids = new List<FlockingAgent>();
        boids = GameObject.FindObjectOfType<FlockingAgentManager>().GetUnitsInRangeOfUnit(agent, radius);
        Vector2 steering = Vector2.zero;

        if (boids.Count == 0)
            return Vector2.zero;

        foreach (FlockingAgent b in boids)
        {
            
            float distance = Vector2.Distance(position, b.transform.position);
            steering += ((Vector2)b.transform.position - position);
            //pointToSeparateFrom += ((position - (Vector2)b.transform.position).normalized);

        }
        steering /= boids.Count;
        steering *= -1;
        Debug.DrawRay(position, (steering).normalized, Color.green);

        return steering.normalized;
    }
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, gizmoRadius);
    }
}
