using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CohesionSteer : MonoBehaviour
{

    public float minDistanceToPoint;
    float gizmoRadius;
    public Vector2 GetSteering(Vector2 position, FlockingAgent agent, float radius)
    {
        gizmoRadius = radius;
        List<FlockingAgent> boids = new List<FlockingAgent>();
        boids = GameObject.FindObjectOfType<FlockingAgentManager>().GetUnitsInRangeOfUnit(agent, radius);
        Vector2 pointToSeparateFrom = Vector2.zero;
        Vector2 avgPos = Vector2.zero;
        if (boids.Count == 0)
            return Vector2.zero;

        foreach (FlockingAgent b in boids)
        {
            pointToSeparateFrom += ((position - (Vector2)b.transform.position).normalized);
            avgPos += (Vector2)b.transform.position;
        }
        avgPos /= boids.Count;

        //Debug.DrawRay(position, (avgPos - position).normalized, Color.green);
        return (avgPos - position).normalized;

    }
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(transform.position, gizmoRadius);
    }
}
