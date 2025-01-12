using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FSM : MonoBehaviour
{
    public GameObject[] treasures;
    public GameObject exit;
    public Transform car;
    public float detectionRadius = 3f;
    public float stealDistance = 2f;
    public float safeDistance = 10f;
    public float loseDistance = 1.5f;

    private UnityEngine.AI.NavMeshAgent agent;
    private WaitForSeconds wait = new WaitForSeconds(0.05f);
    private delegate IEnumerator State();
    private State state;
    private State previouseState;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        state = Phase1;
        StartCoroutine(StateMachine());
    }

    IEnumerator StateMachine()
    {
        while (enabled)
            yield return StartCoroutine(state());
    }

    // Fase 1: Recolectar tesoros y moverse a la salida
    IEnumerator Phase1()
    {
        previouseState = Phase1;
        Debug.Log("State: Phase1 (Search Supplies)");

        while (true)
        {
            if (IsDetected())
            {
                state = Evade;
                yield break;
            }

            // Buscar el tesoro más cercano
            GameObject nearestTreasure = FindNearest(treasures);

            if (nearestTreasure != null)
            {
                agent.SetDestination(nearestTreasure.transform.position);

                // Dirigirse al tesoro
                while (Vector3.Distance(transform.position, nearestTreasure.transform.position) > stealDistance)
                {
                    if (IsDetected())
                    {
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
                // Si no quedan tesoros, dirigirse a la salida
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

                Debug.Log("Exit reached. Starting Phase2.");
                GameManager.Instance.StartPhase2();
                agent.enabled = false;
                transform.position = GameManager.Instance.spawnPointPhase2.position;
                agent.enabled = true;
                state = Phase2;
                yield break;
            }
        }
    }

    // Fase 2: Ir al coche y ganar
    IEnumerator Phase2()
    {
        previouseState = Phase2;
        Debug.Log("State: Phase2 (Go to Car)");

        while (true)
        {
            if (IsDetected())
            {
                state = Evade;
                yield break;
            }

            // Dirigirse al coche
            if (car != null)
            {
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
                GameManager.Instance.WinGame();
                enabled = false;
                yield break;
            }
            else
            {
                Debug.LogWarning("Car target is missing.");
            }

            yield return wait;
        }
    }

    // Evitar enemigos
    IEnumerator Evade()
    {
        Debug.Log("State: Evade");

        while (true)
        {
            if (!IsDetected())
            {
                Debug.Log("Evaded successfully. Returning to previous state.");
                if (previouseState == Phase2)
                {
                    state = Phase2; // Regresa a Phase2
                }
                else
                {
                    state = Phase1; // Regresa a Phase1 por defecto
                }
                yield break;
            }

            // Calcular dirección opuesta a los enemigos detectados
            List<GameObject> detectingEnemies = new List<GameObject>();
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (IsEnemyDetecting(enemy))
                {
                    detectingEnemies.Add(enemy);
                }
            }

            if (detectingEnemies.Count > 0)
            {
                Vector3 evadeDirection = Vector3.zero;
                foreach (GameObject enemy in detectingEnemies)
                {
                    evadeDirection += (transform.position - enemy.transform.position).normalized;
                }

                evadeDirection /= detectingEnemies.Count;
                Vector3 evadePoint = transform.position + evadeDirection * safeDistance;

                agent.SetDestination(evadePoint);
            }

            yield return wait;
        }
    }

    // Detecta si el enemigo está cerca o visible
    private bool IsEnemyDetecting(GameObject enemy)
    {
        Camera visionCamera = enemy.GetComponent<Camera>();
        if (visionCamera != null && IsVisibleToCamera(visionCamera))
        {
            return true;
        }

        float distance = Vector3.Distance(transform.position, enemy.transform.position);
        return distance < detectionRadius;
    }

    // Determina si el jugador es detectado
    private bool IsDetected()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (IsEnemyDetecting(enemy))
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < loseDistance)
                {
                    Debug.Log("Game Over! Too close to an enemy.");
                    GameManager.Instance.LoseGame();
                    enabled = false;
                    return true;
                }
                return true;
            }
        }
        return false;
    }

    // Verifica si el jugador es visible para la cámara del enemigo
    private bool IsVisibleToCamera(Camera camera)
    {
        Vector3 viewportPoint = camera.WorldToViewportPoint(transform.position);
        return viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1;
    }

    // Encuentra el objeto más cercano en una lista
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
}
