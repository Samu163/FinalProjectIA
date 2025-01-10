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

    private int currentWaypointIndex;
    private NavMeshAgent agent;
    private Transform targetRobber;   // Referencia al "robber" detectado
    private float lostSightTimer;
    private enum AIState { Patrolling, Chasing }
    private AIState currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = AIState.Patrolling;

        // Configura la velocidad inicial para patrullaje
        agent.speed = patrolSpeed;

        // Inicia la patrulla
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        StartCoroutine(Patrol());
    }

    void Update()
    {
        if (currentState == AIState.Patrolling)
        {
            DetectRobber();
        }
        else if (currentState == AIState.Chasing)
        {
            ChaseRobber();
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
        if (visionCamera == null)
        {
            Debug.LogError("La cámara de visión no está asignada.");
            return;
        }

        // Encuentra todos los objetos con la etiqueta "robber"
        GameObject[] robbers = GameObject.FindGameObjectsWithTag("robber");

        foreach (GameObject robber in robbers)
        {
            // Convierte la posición del "robber" al espacio de la pantalla de la cámara
            Vector3 viewportPoint = visionCamera.WorldToViewportPoint(robber.transform.position);

            // Verifica si el "robber" está dentro del campo de visión (entre 0 y 1 en la pantalla)
            if (viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1)
            {
                // Realiza un raycast para asegurarte de que no hay obstáculos entre la cámara y el "robber"
                Ray ray = visionCamera.ViewportPointToRay(new Vector3(viewportPoint.x, viewportPoint.y, 0));
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.CompareTag("robber"))
                    {
                        targetRobber = hit.transform;
                        currentState = AIState.Chasing;
                        lostSightTimer = lostSightTime;

                        // Cambia la velocidad a la de persecución
                        agent.speed = chaseSpeed;

                        break;
                    }
                }
            }
        }
    }

    void ChaseRobber()
    {
        if (targetRobber == null)
        {
            currentState = AIState.Patrolling;

            // Cambia la velocidad a la de patrullaje
            agent.speed = patrolSpeed;

            StartCoroutine(Patrol());
            return;
        }

        // Persigue al "robber"
        agent.SetDestination(targetRobber.position);

        // Verifica si el "robber" todavía está en el campo de visión
        Vector3 viewportPoint = visionCamera.WorldToViewportPoint(targetRobber.position);
        if (viewportPoint.z <= 0 || viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1)
        {
            lostSightTimer -= Time.deltaTime;
            if (lostSightTimer <= 0)
            {
                // Si pierde de vista al "robber", vuelve a patrullar
                targetRobber = null;
                currentState = AIState.Patrolling;

                // Cambia la velocidad a la de patrullaje
                agent.speed = patrolSpeed;

                StartCoroutine(Patrol());
            }
        }
        else
        {
            // Si el "robber" está en vista, reinicia el temporizador
            lostSightTimer = lostSightTime;
        }
    }

    void OnDrawGizmos()
    {
        // Opcional: Dibuja un frustum para visualizar el campo de visión de la cámara
        if (visionCamera != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.matrix = visionCamera.transform.localToWorldMatrix;
            Gizmos.DrawFrustum(Vector3.zero, visionCamera.fieldOfView, visionCamera.farClipPlane, visionCamera.nearClipPlane, visionCamera.aspect);
        }
    }
}


