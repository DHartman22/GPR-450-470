using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using System.Linq;

public class CheckForGuardInRange : Node
{
    private Transform transform;

    public CheckForGuardInRange(Transform transform)
    {
        this.transform = transform;
    }

    public override NodeState Evaluate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, Guard.range, 1 << 8);
        object t = GetData("Target");
        parent.parent.SetData("selfGuard", transform.GetComponent<Guard>());
        if (t == null)
        {
            List<Collider> list = new List<Collider>();
            //order by distance from original
            Collider[] sortedColliders = colliders.OrderBy(c => (transform.position - c.transform.position).magnitude).ToArray();
            bool guardFound = false;
            if(sortedColliders.Length > 1)
            {
                int index = -1;
                for (index = 0; index < sortedColliders.Length; index++)
                {
                    //Prevents following known unconscious guards
                    if(!sortedColliders[1].transform.GetComponent<Guard>().discovered)
                    {
                        //Can the guard see it?
                        if (!Physics.Raycast(new Ray(transform.position, sortedColliders[index].transform.position),
                            Vector3.Distance(transform.position, sortedColliders[index].transform.position), 1 << 6))
                        {
                            guardFound = true;
                            Debug.DrawLine(sortedColliders[index].transform.position, transform.position, Color.cyan);

                            parent.parent.SetData("target", sortedColliders[1].transform);
                            parent.parent.SetData("targetGuard", sortedColliders[1].transform.GetComponent<Guard>());
                            break;
                        }
                    }
                }
                if(!guardFound)
                {
                    state = NodeState.FAILURE;
                    return state;
                }
                
                Guard self = (Guard)parent.parent.GetData("selfGuard");
                Guard targetGuard = (Guard)parent.parent.GetData("targetGuard");
                
                if (targetGuard.unconscious && targetGuard.discovered == false)
                {
                    targetGuard.discovered = true;
                    Guard.minimumFear += 10f;
                    
                }
                if (self.fear >= Guard.seekAlliesFearRequirement)
                {
                    self.mode = Guard.Mode.SeekGuard;
                    state = NodeState.SUCCESS;
                    return state;

                }
                else
                {
                    state = NodeState.FAILURE;
                    return state;
                }
            }

            state = NodeState.FAILURE;
            return state;
        }

        state = NodeState.SUCCESS;
        return state;
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
