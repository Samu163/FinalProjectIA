using UnityEngine;
using System.Collections;

public class FSM : MonoBehaviour
{
    public GameObject[] cops;
    public GameObject[] treasures;
    public GameObject exit;
    public Transform[] civilians;
    public Transform car;
    public float detectionRadius = 15f;
    public float stealDistance = 2f;
    public float safeDistance = 10f;
    public string statename = "Search Supplies";
    public static FSM Instance;

    private UnityEngine.AI.NavMeshAgent agent;
    private WaitForSeconds wait = new WaitForSeconds(0.05f);
    private delegate IEnumerator State();
    private State state;

    void Start()
    {
        Instance = this;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        state = SearchSupplies;
        StartCoroutine(StateMachine());
    }

    public void Update()
    {
        statename = state.Method.Name;
    }

    IEnumerator StateMachine()
    {
        while (enabled)
            yield return StartCoroutine(state());
    }

    IEnumerator SearchSupplies()
    {
        Debug.Log("State: Search Supplies");

        while (true)
        {
            GameObject nearestTreasure = FindNearest(treasures);

            if (nearestTreasure != null)
            {
                agent.SetDestination(nearestTreasure.transform.position);

                while (Vector3.Distance(transform.position, nearestTreasure.transform.position) > stealDistance)
                {
                    if (IsDetected())
                    {
                        state = Evade;
                        yield break;
                    }
                    yield return wait;
                }

                // Treasure reached
                Debug.Log("Treasure collected!");
                nearestTreasure.SetActive(false);
            }
            else
            {
                // No more treasures, move to exit
                agent.SetDestination(exit.transform.position);

                while (Vector3.Distance(transform.position, exit.transform.position) > stealDistance)
                {
                    if (IsDetected())
                    {
                        state = Evade;
                        yield break;
                    }
                    yield return wait;
                }

                // Reached exit
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

        agent.SetDestination(GameManager.Instance.spawnPointPhase2.position);

        yield return null;
        state = EscapeOrRescue;
    }


    IEnumerator EscapeOrRescue()
    {
        Debug.Log("State: Escape or Rescue");

        while (true)
        {
            Transform nearestCivilian = FindNearest(civilians);

            if (nearestCivilian != null && Vector3.Distance(transform.position, nearestCivilian.position) < detectionRadius)
            {
                agent.SetDestination(nearestCivilian.position);

                while (Vector3.Distance(transform.position, nearestCivilian.position) > stealDistance)
                {
                    if (IsDetected())
                    {
                        state = Evade;
                        yield break;
                    }
                    yield return wait;
                }

                Debug.Log("Civilian rescued!");
            }
            else
            {
                // No civilians nearby, go to car
                agent.SetDestination(car.position);

                while (Vector3.Distance(transform.position, car.position) > stealDistance)
                {
                    if (IsDetected())
                    {
                        state = Evade;
                        yield break;
                    }
                    yield return wait;
                }

                Debug.Log("Car reached. You win!");
                yield break;
            }
        }
    }

    IEnumerator Evade()
    {
        Debug.Log("State: Evade");

        while (IsDetected())
        {
            Vector3 safePoint = FindNearestSafePoint();
            agent.SetDestination(safePoint);
            yield return wait;
        }

        Debug.Log("Evaded successfully. Returning to previous task.");
        state = SearchSupplies;
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

    private Vector3 FindNearestSafePoint()
    {
        // Placeholder: return a random point at a safe distance
        return transform.position + Random.insideUnitSphere * safeDistance;
    }

    private bool IsDetected()
    {
        for (int i = 0; i < cops.Length; i++)
        {
            if (Vector3.Distance(transform.position, cops[i].transform.position) < detectionRadius)
            {
                return true;
            }
        }
        for (int i = 0; i < GameManager.Instance.zombieSpawner.zombies.Count; i++)
        {
            if (Vector3.Distance(transform.position, GameManager.Instance.zombieSpawner.zombies[i].transform.position) < detectionRadius)
            {
                return true;
            }
        }
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Zombie"))
        {
            Debug.Log("Player caught by zombie. Game over!");
            // Implementation for game over
        }
        if (collision.gameObject.CompareTag("Police"))
        {
            Debug.Log("Player caught by police. Game over!");
            // Implementation for game over
        }
    }
}
