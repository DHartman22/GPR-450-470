using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;
using System.Linq;

public class CheckForLights : Node
{
    private Transform transform;

    public CheckForLights(Transform transform)
    {
        this.transform = transform;
    }

    public override NodeState Evaluate()
    {
        if(transform.GetComponent<Guard>().fear < Guard.seekLightFearRequirement)
        {
            state = NodeState.FAILURE;
            return state;
        }
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, Guard.range, 1 << 10);
        Collider[] agents = Physics.OverlapSphere(transform.position, Guard.range, 1 << 8);

        object t = GetData("Target");

        parent.parent.SetData("selfGuard", transform.GetComponent<Guard>());


        if (t == null)
        {
            List<GridCell> list = new List<GridCell>();
            foreach(Collider c in colliders)
            {
                list.Add(c.GetComponent<GridCell>());
            }
            List<GridCell> sortedCells = new List<GridCell>();
            sortedCells.AddRange(list.OrderBy(c => (transform.position - c.transform.position).magnitude).ToArray());

            if (sortedCells.Count > 1)
            {
                float highestInfluence = 0;
                int index;
                int highestIndex = -1;
                for(index = 0; index < sortedCells.Count; index++)
                {
                    
                    if(sortedCells[index].influence > 0)
                    {
                        //Can the guard see it?
                        //if(!Physics.Raycast(new Ray(transform.position, sortedCells[index].transform.position), 
                        //    Vector3.Distance(transform.position, sortedCells[index].transform.position)
                        //    , 1 << 6))
                        //{
                            highestIndex = index;
                            highestInfluence = sortedCells[index].influence;
                            Debug.DrawLine(sortedCells[index].transform.position, transform.position, Color.cyan);
                            break;
                        //}
                    }
                }
                if(highestIndex == -1)
                {
                    state = NodeState.FAILURE;
                    return state;
                }
                parent.parent.SetData("target", sortedCells[highestIndex].transform);
                //parent.parent.SetData("targetGuard", colliders[1].transform.GetComponent<Guard>());

                Guard self = (Guard)parent.parent.GetData("selfGuard");

                if (self.fear >= Guard.seekLightFearRequirement && highestInfluence > 0 && self.mode != Guard.Mode.SeekGuard)
                {
                    self.mode = Guard.Mode.SeekLight;
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
