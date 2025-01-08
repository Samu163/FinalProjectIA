using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wander : MonoBehaviour
{
    public NavMeshAgent agent;
    public float wanderRadius = 10f;
    public float detectionRange = 20f;
    public float fieldOfView = 60f;

    private Vector3 wanderTarget;
   
    void Update()
    {
        Wandering();
    }

    void Wandering()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            SetRandomWanderTarget();
        }
    }
    void SetRandomWanderTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            wanderTarget = hit.position;
            agent.SetDestination(wanderTarget);
        }
    }
}
