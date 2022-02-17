using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignmentSteer : MonoBehaviour
{
    float gizmoRadius;

    public Vector2 GetSteering(Vector2 position, FlockingAgent agent, float radius)
    {
        gizmoRadius = radius;
        List<FlockingAgent> boids = new List<FlockingAgent>();
        boids = GameObject.FindObjectOfType<FlockingAgentManager>().GetUnitsInRangeOfUnit(agent, radius);
        Vector2 pointToSeparateFrom = Vector2.zero;
        Vector2 avgVel = Vector2.zero;

        if (boids.Count == 0)
            return Vector2.zero;

        foreach (FlockingAgent b in boids)
        {
            avgVel += b.velocity;
        }
        avgVel /= boids.Count;

        //Debug.DrawRay(position, (avgVel).normalized, Color.yellow);
        return (avgVel).normalized;

    }
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(transform.position, gizmoRadius);
    }
}
