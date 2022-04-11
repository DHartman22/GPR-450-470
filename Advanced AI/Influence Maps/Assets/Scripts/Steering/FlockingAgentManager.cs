using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingAgentManager : MonoBehaviour
{
    public List<FlockingAgent> agents;
    public float seekFleeWeight;
    public float cohesionWeight;
    public float separationWeight;
    public float alignmentWeight;
    public float obstacleAvoidanceWeight;
    public SeekFleeSteer seekFlee;
    public SeparationSteer separation;
    public CohesionSteer cohesion;
    public AlignmentSteer alignment;
    public FlowFieldSteer flow;
    public InfluenceSteer influence;
    public float alignmentRadius;
    public float cohesionRadius;
    public float separationRadius;
    public Transform spawnOne;
    public Transform spawnTwo;
    public Transform spawnThree;

    public GameObject agentObject;

    // Start is called before the first frame update

    //Effectively a neighborhood implementation
    public List<FlockingAgent> GetUnitsInRangeOfUnit(FlockingAgent agent, float radius)
    {
        List<FlockingAgent> agentsInRadius = new List<FlockingAgent>();
        for(int i = 0; i < agents.Count; i++)
        {
            if(agents[i] != agent && Vector3.Distance(agent.transform.position, agents[i].transform.position) < radius)
            {
                agentsInRadius.Add(agents[i]);
            }
        }
        return agentsInRadius;
    }

    public Vector2 GetSteering(FlockingAgent agent)
    {
        Vector2 totalSteering = Vector2.zero;
        //Vector2 obstacleAvoidance = agent.GetObstacleAvoidanceSteering() * obstacleAvoidanceWeight;
        //if (obstacleAvoidance == Vector2.zero)
        //{
              totalSteering += separation.GetSteering(agent.transform.position, agent, separationRadius) * separationWeight;
        //    totalSteering += cohesion.GetSteering(agent.transform.position, agent, cohesionRadius) * cohesionWeight;
        //    totalSteering += alignment.GetSteering(agent.transform.position, agent, alignmentRadius) * alignmentWeight;
        //    totalSteering += seekFlee.GetSteering(agent.transform.position, agent.velocity) * seekFleeWeight;
        //}
        //else
        //    totalSteering += obstacleAvoidance;

        //totalSteering += flow.GetSteering(agent.transform.position, agent.velocity, agent);
        totalSteering += influence.GetSteering(agent.transform.position, agent.velocity, agent);


        return totalSteering;
    }

    void SpawnNewAgent()
    {
        GameObject newAgent;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            newAgent = Instantiate(agentObject, spawnOne.position, Quaternion.identity, null);
            agents.Add(newAgent.GetComponent<FlockingAgent>());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            newAgent = Instantiate(agentObject, spawnTwo.position, Quaternion.identity, null);
            agents.Add(newAgent.GetComponent<FlockingAgent>());
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            newAgent = Instantiate(agentObject, spawnThree.position, Quaternion.identity, null);
            agents.Add(newAgent.GetComponent<FlockingAgent>());
        }
    }

    void Start()
    {
        agents = new List<FlockingAgent>();
        agents.AddRange(GameObject.FindObjectsOfType<FlockingAgent>());
    }

    // Update is called once per frame
    void Update()
    {
        SpawnNewAgent();
    }
}
