using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public class FSM : MonoBehaviour
{
    public GameObject[] treasures;
    public GameObject exit;
    public Transform[] civilians;
    public Transform car;
    public float detectionRadius = 3f;
    public float stealDistance = 2f;
    public float safeDistance = 10f;
    public float loseDistance = 0.5f;
    public string statename = "Search Supplies";
    public static FSM Instance;
    private UnityEngine.AI.NavMeshAgent agent;
    private WaitForSeconds wait = new WaitForSeconds(0.05f);
    private delegate IEnumerator State();
    private State state;
    private State previousState; // Nuevo: Almacena el estado previo

    void Start()
    {
        Instance = this;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        state = Phase1;
        StartCoroutine(StateMachine());
    }

    void Update()
    {
        statename = state.Method.Name;
    }

    IEnumerator StateMachine()
    {
        while (enabled)
            yield return StartCoroutine(state());
    }

    IEnumerator Phase1()
    {
        Debug.Log("State: Search Supplies");

        while (true)
        {
            if (IsDetected())
            {
                previousState = Phase1; // Guardar el estado actual
                state = Evade;
                yield break;
            }

            GameObject nearestTreasure = FindNearest(treasures);

            if (nearestTreasure != null)
            {
                agent.SetDestination(nearestTreasure.transform.position);

                while (Vector3.Distance(transform.position, nearestTreasure.transform.position) > stealDistance)
                {
                    if (IsDetected())
                    {
                        previousState = Phase1;
                        state = Evade;
                        yield break;
                    }
                    yield return wait;
                }

                Debug.Log("Treasure collected!");
                nearestTreasure.SetActive(false);
            }
            else
            {
                agent.SetDestination(exit.transform.position);

                while (Vector3.Distance(transform.position, exit.transform.position) > stealDistance)
                {
                    if (IsDetected())
                    {
                        previousState = Phase1;
                        state = Evade;
                        yield break;
                    }
                    yield return wait;
                }

                Debug.Log("Exit reached. Spawning zombies and starting Phase 2.");
                state = GoToPhase2;
                GameManager.Instance.StartPhase2();
                yield break;
            }
        }
    }

    IEnumerator GoToPhase2()
    {
        Debug.Log("State: GoToPhase2");

        agent.enabled = false;
        transform.position = GameManager.Instance.spawnPointPhase2.position;
        agent.enabled = true;
        yield return null;

        state = Phase2;
    }

    IEnumerator Phase2()
    {
        Debug.Log("State: Escape or Rescue");

        while (true)
        {

            if (IsDetected())
            {
                previousState = Phase1; // Guardar el estado actual
                state = Evade;
                yield break;
            }
            Transform nearestCivilian = FindNearest(civilians);

            if (nearestCivilian != null && Vector3.Distance(transform.position, nearestCivilian.position) < detectionRadius)
            {
                agent.SetDestination(nearestCivilian.position);

                while (Vector3.Distance(transform.position, nearestCivilian.position) > stealDistance)
                {
                    if (IsDetected())
                    {
                        previousState = Phase2; // Guardar el estado actual
                        state = Evade;
                        yield break;
                    }
                    yield return wait;
                }

                Debug.Log("Civilian rescued!");
            }
            else
            {
                agent.SetDestination(car.position);

                while (Vector3.Distance(transform.position, car.position) > stealDistance)
                {
                    if (IsDetected())
                    {
                        previousState = Phase2; // Guardar el estado actual
                        state = Evade;
                        yield break;
                    }
                    yield return wait;
                }

                Debug.Log("Car reached. You win!");
                GameManager.Instance.WinGame();
                yield break;
            }
        }
    }

    IEnumerator Evade()
    {
        Debug.Log("State: Evade");

        while (true)
        {
            if (!IsDetected())
            {
                previousState = Phase1; // Guardar el estado actual
                state = Evade;
                yield break;
            }
           
            List<GameObject> detectingEnemies = new List<GameObject>();

            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                PatrollingAI ai = enemy.GetComponent<PatrollingAI>();
                if (ai != null && ai.visionCamera != null && IsVisibleToCamera(ai.visionCamera))
                {
                    detectingEnemies.Add(enemy);
                }
            }

            if (detectingEnemies.Count == 0)
            {
                Debug.Log("Evaded successfully. Returning to previous state.");
                state = previousState; // Regresa al estado previo
                yield break;
            }

            Vector3 evadeDirection = Vector3.zero;
            foreach (GameObject enemy in detectingEnemies)
            {
                evadeDirection += (transform.position - enemy.transform.position).normalized;
            }

            evadeDirection /= detectingEnemies.Count;
            Vector3 evadePoint = transform.position + evadeDirection * safeDistance;

            agent.SetDestination(evadePoint);
            yield return wait;
        }
    }

    private GameObject FindNearest(GameObject[] objects)
    {
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject obj in objects)
        {
            if (obj.activeSelf)
            {
                float distance = Vector3.Distance(transform.position, obj.transform.position);
                if (distance < minDistance)
                {
                    nearest = obj;
                    minDistance = distance;
                }
            }
        }

        return nearest;
    }

    private Transform FindNearest(Transform[] transforms)
    {
        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform t in transforms)
        {
            float distance = Vector3.Distance(transform.position, t.position);
            if (distance < minDistance)
            {
                nearest = t;
                minDistance = distance;
            }
        }

        return nearest;
    }

    private bool IsDetected()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Camera visionCamera = enemy.GetComponent<Camera>();
            if (visionCamera != null)
            {
                if (IsVisibleToCamera(visionCamera))
                {
                    return true;
                }
            }
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < loseDistance) // Comprobación de derrota
            {
                Debug.Log("Game Over! Too close to an enemy.");
                GameManager.Instance.LoseGame(); 
                StopAllCoroutines();
                enabled = false;
                return true;
            }

            if (Vector3.Distance(transform.position, enemy.transform.position) < detectionRadius)
            {
                return true;
            }
           
        }

        return false;
    }

    private bool IsVisibleToCamera(Camera camera)
    {
        Vector3 viewportPoint = camera.WorldToViewportPoint(transform.position);
        return viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1;
    }


    
}
