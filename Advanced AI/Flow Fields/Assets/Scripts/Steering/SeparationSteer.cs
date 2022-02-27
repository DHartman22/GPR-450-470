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
            if(!b.isObstacleAvoiding)
            {
                steering += ((Vector2)b.transform.position - position);
            }
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
