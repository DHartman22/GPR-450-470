using System.Collections;
using System.Collections.Generic;


using BehaviorTree;
using UnityEngine.AI;

public class Guard : Tree
{
    public float fear;
    public UnityEngine.UI.Slider fearMeter;
    public float fearDecayPerSecond;
    public float fearDecayDelay;
    public float timeSinceLastFearEvent;
    public UnityEngine.Transform[] waypoints;
    public static float speed = 2f;
    [UnityEngine.SerializeField] public static float arrivedDistance = 6f;
    public NavMeshAgent agent;
    protected override Node SetupTree()
    {
        Node root = new TaskPatrol(transform, waypoints, agent);
        return root;
    }
    
    public void FearEvent(float fearToAdd)
    {
        fear += fearToAdd;
        timeSinceLastFearEvent = 0;
    }

    private void OnMouseDown()
    {
        FearEvent(10);
    }

    private void FixedUpdate()
    {
        timeSinceLastFearEvent += UnityEngine.Time.fixedDeltaTime;
        fearMeter.value = fear;
        if(timeSinceLastFearEvent > fearDecayDelay)
            fear -= UnityEngine.Time.fixedDeltaTime / fearDecayPerSecond;
    }
}
