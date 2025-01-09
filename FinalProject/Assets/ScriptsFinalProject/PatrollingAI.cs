using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrollingAI : MonoBehaviour
{
    public Transform[] waypoints;  
    public float waitTime = 1f;    
    private int currentWaypointIndex;
    private NavMeshAgent agent;    

    void Start()
    {
        
        agent = GetComponent<NavMeshAgent>();

        agent.SetDestination(waypoints[currentWaypointIndex].position);

        
        StartCoroutine(Patrol());
    }

    IEnumerator Patrol()
    {
        while (true)
        {
            
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                yield return new WaitForSeconds(waitTime);

                
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }

            yield return null;
        }
    }
}
