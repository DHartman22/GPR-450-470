using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;

public class TaskPatrol : Node
{
    private Transform _transform;
    private List<Transform> _waypoints;
    private NavMeshAgent _agent;

    private float arriveDistance = 2f;
    [SerializeField] int targetWaypointIndex = 0;
    bool reversed = false;
    public TaskPatrol(Transform transform, Transform[] waypoints, NavMeshAgent agent)
    {
        _transform = transform;
        _waypoints = new List<Transform>();
        _waypoints.AddRange(waypoints);
        _agent = agent;

    }

    public override NodeState Evaluate()
    {
        if(_agent.destination != _waypoints[targetWaypointIndex].position)
            _agent.SetDestination(_waypoints[targetWaypointIndex].position);
        
        if(Vector3.Distance(_transform.position, _agent.destination) < Guard.arrivedDistance)
        {
            if(reversed)
            {
                targetWaypointIndex--;
                if (targetWaypointIndex < 0)
                {
                    targetWaypointIndex += 2;
                    reversed = false;
                }
            }
            else
            {
                targetWaypointIndex++;
                if (targetWaypointIndex >= _waypoints.Count)
                {
                    targetWaypointIndex -= 2;
                    reversed = true;
                }
            }
        }
        Debug.Log(_transform.name + " Distance: " + Vector3.Distance(_transform.position, _agent.destination));

        state = NodeState.RUNNING;
        return state;
    }
}
