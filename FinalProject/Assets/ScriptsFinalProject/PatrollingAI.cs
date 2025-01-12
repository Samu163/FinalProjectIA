using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PatrollingAI : MonoBehaviour
{
    public Transform[] waypoints;      // Waypoints para patrullar
    public float waitTime = 1f;        // Tiempo de espera en cada waypoint
    public Camera visionCamera;       // Cámara usada como campo de visión
    public float lostSightTime = 3f;  // Tiempo antes de volver a patrullar si pierde de vista al "robber"

    public float patrolSpeed = 3.5f;  // Velocidad durante la patrulla
    public float chaseSpeed = 6f;     // Velocidad durante la persecución

    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;
    private Transform targetRobber;   // Referencia al "robber" detectado
    private float lostSightTimer = 0f;
    private enum AIState { Patrolling, Chasing }
    private AIState currentState = AIState.Patrolling;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;

        // Inicia el patrullaje
        StartPatrolling();
    }

    void Update()
    {
        switch (currentState)
        {
            case AIState.Patrolling:
                DetectRobber();
                break;
            case AIState.Chasing:
                ChaseRobber();
                break;
        }
    }

    void StartPatrolling()
    {
        currentState = AIState.Patrolling;
        agent.speed = patrolSpeed;

        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
            StartCoroutine(Patrol());
        }
    }

    IEnumerator Patrol()
    {
        while (currentState == AIState.Patrolling)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                yield return new WaitForSeconds(waitTime);

                // Avanza al siguiente waypoint
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }

            yield return null;
        }
    }

    void DetectRobber()
    {
        if (visionCamera == null) return;

        GameObject[] robbers = GameObject.FindGameObjectsWithTag("robber");

        foreach (GameObject robber in robbers)
        {
            Vector3 viewportPoint = visionCamera.WorldToViewportPoint(robber.transform.position);

            if (viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1)
            {
                Ray ray = visionCamera.ViewportPointToRay(viewportPoint);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.CompareTag("robber"))
                    {
                        targetRobber = hit.transform;
                        lostSightTimer = lostSightTime;
                        StartChasing();
                        return;
                    }
                }
            }
        }
    }

    void StartChasing()
    {
        currentState = AIState.Chasing;
        agent.speed = chaseSpeed;
    }

    void ChaseRobber()
    {
        if (targetRobber == null)
        {
            StopChasing();
            return;
        }

        agent.SetDestination(targetRobber.position);

        Vector3 viewportPoint = visionCamera.WorldToViewportPoint(targetRobber.position);
        if (viewportPoint.z <= 0 || viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1)
        {
            lostSightTimer -= Time.deltaTime;
            if (lostSightTimer <= 0)
            {
                StopChasing();
            }
        }
        else
        {
            lostSightTimer = lostSightTime;
        }
    }

    void StopChasing()
    {
        targetRobber = null;
        StartPatrolling();
    }

    void OnDrawGizmos()
    {
        if (visionCamera != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.matrix = visionCamera.transform.localToWorldMatrix;
            Gizmos.DrawFrustum(Vector3.zero, visionCamera.fieldOfView, visionCamera.farClipPlane, visionCamera.nearClipPlane, visionCamera.aspect);
        }
    }
}
