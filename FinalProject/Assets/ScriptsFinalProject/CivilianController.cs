using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CivilianController : MonoBehaviour
{
    public Transform player; 
    public float detectionRadius = 10f; 
    public float neighbourDistance = 3f; 
    public float separationDistance = 1.5f; 
    public float cohesionWeight = 1f; 
    public float separationWeight = 1.5f; 
    public float alignmentWeight = 1f; 

    private bool isFollowing = false; 
    private CivilianController[] allCivilians; 
    private NavMeshAgent agent; 
    private Vector3 flockingDirection; 

    void Start()
    {
        allCivilians = FindObjectsOfType<CivilianController>();
        agent = GetComponent<NavMeshAgent>(); 
        flockingDirection = Vector3.zero;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!isFollowing && distanceToPlayer <= detectionRadius)
        {
            isFollowing = true;
        }

        if (isFollowing)
        {
            FollowPlayerWithFlocking();
        }
    }

    private void FollowPlayerWithFlocking()
    {
        Vector3 cohesion = Vector3.zero; 
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero; 
        int groupSize = 0;

        foreach (var civil in allCivilians)
        {
            if (civil != this)
            {
                float distance = Vector3.Distance(civil.transform.position, transform.position);

                if (distance <= neighbourDistance)
                {
                    // Cohesion
                    cohesion += civil.transform.position;

                    // Separation
                    if (distance < separationDistance)
                    {
                        separation += transform.position - civil.transform.position;
                    }

                    // Aligment
                    alignment += civil.agent.velocity;
                    groupSize++;
                }
            }
        }

        if (groupSize > 0)
        {
            cohesion = (cohesion / groupSize - transform.position).normalized * cohesionWeight;
            separation = separation.normalized * separationWeight;
            alignment = (alignment / groupSize).normalized * alignmentWeight;
        }

        Vector3 follow = (player.position - transform.position).normalized;
        flockingDirection = follow + cohesion + separation + alignment;
        flockingDirection = flockingDirection.normalized;

        agent.SetDestination(transform.position + flockingDirection);
    }
}
