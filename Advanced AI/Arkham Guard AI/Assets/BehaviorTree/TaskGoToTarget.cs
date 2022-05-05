using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;
public class TaskGoToTarget : Node
{
    private Transform transform;

    public TaskGoToTarget(Transform transform)
    {
        this.transform = transform;

    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        NavMeshAgent agent = (NavMeshAgent)GetData("agent");
        Guard targetGuard = (Guard)GetData("targetGuard");
        
        if(Vector3.Distance(transform.position, target.position) > 4f)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else if(targetGuard != null)
        {
            agent.isStopped = true;
            if(targetGuard.unconscious && targetGuard.discovered == false)
            {
                targetGuard.discovered = true;
                Guard.minimumFear += 10f;
                
            }    
        }

        state = NodeState.RUNNING;
        return state;
    }
}
