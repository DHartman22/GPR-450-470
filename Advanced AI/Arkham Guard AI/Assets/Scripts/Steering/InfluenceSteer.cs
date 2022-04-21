using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceSteer : MonoBehaviour
{
    public InfluenceMapGrid influenceMap;
    public Vector2 GetSteering(Vector2 position, Vector2 velocity, FlockingAgent agent)
    {
        GridCell targetCell = influenceMap.GetHighestInfluenceCellInRange(agent);
        agent.target = targetCell.transform.position;
        if(targetCell == influenceMap.WorldSpaceToCell(position))
        {
            return Vector2.zero;
        }
        Vector2 desiredVel =  (targetCell.transform.position - agent.transform.position).normalized;
        
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
