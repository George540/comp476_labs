using UnityEngine;

using System.Collections.Generic;
using System.Numerics;
using Utilities;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class FlockAgent : MonoBehaviour 
{
    public bool debug;
    public Transform target;
    public float neighborRadius = 5;
    public float avoidanceRadius = 3.5f;
    public float cohesionFactor = 1.5f;
    public float avoidanceFactor = 2f;
    public float seekSpeed = 3f;

    private Vector3 movement;
	
	void Update () 
    {
        Collider[] neighbors = GetNeighborContext();
        Cohesion(neighbors);
        Avoidance(neighbors);
        Alignment(neighbors);
        Seek();

        // TODO : steer and align the boid in the direction of the movement
        movement = Vector3.ClampMagnitude(movement, seekSpeed);
        transform.position += movement * Time.deltaTime;

        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 70f * Time.deltaTime);
        }

    }

    public Collider[] GetNeighborContext()
    {
        Collider[] neighbors = Physics.OverlapSphere(transform.position, neighborRadius);

        if (debug)
            DebugUtil.DrawWireSphere(transform.position, Color.Lerp(Color.white, Color.red, neighbors.Length), neighborRadius);

        return neighbors;
    }

    // This is the force that keeps the swarm together.
    void Cohesion(Collider[] neighbors)
    {
        // movement is equal to the relative offset from the flock agent to the center of the flock
        Vector3 cohesiveMovement = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            if (neighbor.transform == transform) continue;

            cohesiveMovement += neighbor.transform.position;
        }

        if (neighbors.Length > 0)
        {
            cohesiveMovement /= neighbors.Length; // average it out
            cohesiveMovement -= transform.position;
        }

        movement += cohesiveMovement.normalized * cohesionFactor;
    }

    // This is the force that dictates the spacing of the swarm.
    void Avoidance(Collider[] neighbors)
    {
        // movement is equal to the average of the sum of all vectors going from neighbor to flock agent within the avoidance radius
        Vector3 avoidanceMovement = Vector3.zero;
        float avoidanceRadSqrd = (avoidanceRadius * avoidanceRadius);
        foreach (var neighbor in neighbors)
        {
            if (neighbor.transform == transform) continue;

            Vector3 neighborToAgent = neighbor.transform.position - transform.position;
            if (Vector3.SqrMagnitude(neighborToAgent) <= avoidanceRadSqrd)
            {
                if (neighborToAgent == Vector3.zero)
                {
                    neighborToAgent = Random.insideUnitSphere * 0.1f;
                }

                avoidanceMovement += neighborToAgent;
            }
        }

        if (neighbors.Length > 0)
        {
            avoidanceMovement =  avoidanceMovement.normalized / neighbors.Length; // average it out
        }

        movement += avoidanceMovement.normalized * avoidanceFactor;

    }

    // This "force" has each of the agents try to sync their orientation.
    void Alignment(Collider[] neighbors)
    {
        // alignedDirection is equal to the average direction of neighbors 
        Vector3 alignedMovement = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            if (neighbor.transform == transform) continue;

            alignedMovement += neighbor.GetComponent<FlockAgent>().movement;
        }

        if (neighbors.Length > 0)
        {
            alignedMovement =  alignedMovement.normalized / neighbors.Length; // average it out
        }

        movement += alignedMovement.normalized;

    }
    
    // Kinematic Seek
    void Seek()
    {
        // TODO
        Vector3 desireVelocity = target.position - transform.position;
        desireVelocity = desireVelocity.normalized * seekSpeed;

        movement += desireVelocity;
    }
}
