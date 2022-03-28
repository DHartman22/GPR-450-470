using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldSteer : MonoBehaviour
{
    public FlowFieldGrid flowField;
    public float maxSpeed;
    public Vector2 GetSteering(Vector2 position, Vector2 velocity, FlockingAgent agent)
    {
        GridCell currentCell = flowField.WorldSpaceToCell(position);
        Vector2 desiredVel = currentCell.direction * maxSpeed;
        if(currentCell.bestCost == 0) //Destination reached
        {
            agent.velocity = Vector2.zero;
            return Vector2.zero;
        }
        return desiredVel;
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
