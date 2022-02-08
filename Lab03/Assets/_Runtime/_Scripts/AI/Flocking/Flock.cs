using UnityEngine;

using System.Collections.Generic;

public class Flock : MonoBehaviour 
{
    public bool debug;
    public int startingFlockCount = 20;
    public GameObject flockAgentPrefab;
    public float neighborRadius = 5;
    public float avoidanceRadius = 3.5f;
    public float cohesionFactor = 1.5f;
    public float avoidanceFactor = 2f;
    public float seekSpeed = 3f;

    private List<FlockAgent> swarm = new List<FlockAgent>();
    private Queue<Transform> targetList = new Queue<Transform>();

    private void Start()
    {
        foreach (GameObject targetObj in GameObject.FindGameObjectsWithTag("Target"))
            targetList.Enqueue(targetObj.transform);

        GenerateSwarm();
        SetNewSwarmTarget();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SetNewSwarmTarget();

        if (Input.GetKey(KeyCode.Q))
            cohesionFactor += Time.deltaTime;
        else if (Input.GetKey(KeyCode.A))
            cohesionFactor -= Time.deltaTime;

        if (Input.GetKey(KeyCode.W))
            avoidanceFactor += Time.deltaTime;
        else if (Input.GetKey(KeyCode.S))
            avoidanceFactor -= Time.deltaTime;

        if (Input.GetKey(KeyCode.E))
            seekSpeed += Time.deltaTime;
        else if (Input.GetKey(KeyCode.D))
            seekSpeed -= Time.deltaTime;

        // synchronize swarm settings across all agents
        foreach (FlockAgent agent in swarm)
        {
            agent.debug = debug;
            agent.neighborRadius = neighborRadius;
            agent.avoidanceRadius = avoidanceRadius;
            agent.cohesionFactor = cohesionFactor;
            agent.avoidanceFactor = avoidanceFactor;
            agent.seekSpeed = seekSpeed;
        }
    }

    private void SetNewSwarmTarget()
    {
        // Managing the Queue. We grab the target, set the swarmlings on it, then requeue it at the back.
        Transform target = targetList.Dequeue();
        foreach (FlockAgent agent in swarm)
        {
            agent.target = target;
        }
        targetList.Enqueue(target);
    }

    private void GenerateSwarm()
    {
        swarm.Clear();
        const float AGENT_DENSITY = 0.08f;
        for (int i = 0; i < startingFlockCount; ++i)
        {
            FlockAgent agent = Instantiate(
                flockAgentPrefab, transform.position + (Random.insideUnitSphere * startingFlockCount * AGENT_DENSITY), 
                Quaternion.identity, 
                transform
            ).GetComponent<FlockAgent>();

            if (agent != null)
                swarm.Add(agent);
        }
    }
}
