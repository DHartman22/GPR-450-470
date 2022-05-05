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
    public float unconsciousTime;
    public float timeSinceLastFearEvent;
    public UnityEngine.Vector3 target;
    public UnityEngine.Transform[] waypoints;
    public static float speed = 2f;
    [UnityEngine.SerializeField] public static float arrivedDistance = 6f;
    public static float range = 40f;
    public static float seekAlliesFearRequirement = 66f;
    public static float seekLightFearRequirement = 33f;
    public List<Guard> guardsFollowing;

    public static float minimumFear = 0f;

    public NavMeshAgent agent;
    public bool unconscious = false;
    public bool discovered = false;
    public Mode mode;
    public UnityEngine.MeshRenderer renderer;
    public UnityEngine.TextMesh text;

    public enum Mode
    {
        Patrolling,
        SeekLight,
        SeekGuard
    }
    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckForGuardInRange(transform),
                new TaskGoToTarget(transform)
            }),
            new Sequence(new List<Node>
            {
                new CheckForLights(transform),
                new TaskGoToTarget(transform)
            }),
            new TaskPatrol(transform, waypoints, agent),
    }) ;
    
        root.SetData("agent", agent);
        return root;
    }
    
    public void FearEvent(float fearToAdd)
    {
        fear += fearToAdd;
        timeSinceLastFearEvent = 0;
    }

    public void UnconsciousEvent()
    {
        unconscious = true;
        timeSinceLastFearEvent = 0;
    }

    public void UpdateColor()
    {
        switch (mode)
        {
            case Mode.Patrolling:
                {
                    renderer.material.color = UnityEngine.Color.green;
                    break;
                }
            case Mode.SeekLight:
                {
                    renderer.material.color = UnityEngine.Color.yellow;

                    break;
                }
            case Mode.SeekGuard:
                {
                    renderer.material.color = UnityEngine.Color.red;

                    break;
                }
        }

    }

    private void FixedUpdate()
    {
        timeSinceLastFearEvent += UnityEngine.Time.fixedDeltaTime;
        fearMeter.value = fear;
        text.text = fear.ToString("F2") + "%";
        if(timeSinceLastFearEvent > fearDecayDelay)
        {
            fear -= fearDecayPerSecond / UnityEngine.Time.fixedDeltaTime;
            if(fear < 0)
                fear = 0;
        }
        if (!unconscious)
        {
            discovered = false;
        }
        if(fear < minimumFear)
        {
            fear = minimumFear;
        }
        if(unconscious)
        {
            agent.isStopped = true;
            text.color = UnityEngine.Color.red;
        }
        if(unconscious && timeSinceLastFearEvent > unconsciousTime)
        {
            fear = 50;
            unconscious = false;
            agent.isStopped = false;
            text.color = UnityEngine.Color.white;

            if (discovered)
                minimumFear -= 10f;
        }
        UpdateColor();
    }

    private void OnDrawGizmos()
    {
        UnityEngine.Gizmos.DrawWireSphere(transform.position, range);
    }

}
