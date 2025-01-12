using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieBehavior : MonoBehaviour
{
    public NavMeshAgent agent;
    public Camera visionCamera;
    public float wanderRadius = 10f;
    public float detectionRange = 20f;
    public float fieldOfView = 60f;
    public float wanderTimer = 5f;

    private Transform player;
    private Vector3 wanderTarget;
    private bool isWandering = true;
    private bool isPursuing = false;
    private float timer;
    private bool ogZombie;
    private BroadcastManager broadcastManager;

    void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        GameObject playerObject = GameObject.FindWithTag("robber");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        broadcastManager = FindObjectOfType<BroadcastManager>();
        if (broadcastManager != null)
        {
            broadcastManager.RegisterZombie(this);
        }

        SetRandomWanderTarget();
        timer = wanderTimer;
    }

    void Update()
    {
        if (player == null) return;

        if (isWandering)
        {
            Wander();
            CheckForPlayer();
        }
        else if (isPursuing)
        {
            PursuePlayer();
            if(ogZombie == true && !CheckForPlayer())
            {
                broadcastManager.OnPlayerLost();
            }
        }
    }

    void Wander()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SetRandomWanderTarget();
            timer = wanderTimer;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            SetRandomWanderTarget();
        }
    }

    bool CheckForPlayer()
    {
        Vector3 directionToPlayer = player.position - visionCamera.transform.position;
        float angleToPlayer = Vector3.Angle(visionCamera.transform.forward, directionToPlayer);

        if (directionToPlayer.magnitude <= detectionRange && angleToPlayer <= fieldOfView / 2)
        {
            if (Physics.Raycast(visionCamera.transform.position, directionToPlayer, out RaycastHit hit, detectionRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    ogZombie = true;
                    isWandering = false;
                    isPursuing = true;
                    broadcastManager.OnPlayerDetected();
                    return true;
                }
            }
        }
        return false;
    }

    void PursuePlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    public void SetPursuing(Transform targetPlayer)
    {
        isWandering = false;
        isPursuing = true;
        player = targetPlayer;
    }

    public void SetWandering()
    {
        isWandering = true;
        isPursuing = false;
        ogZombie = false;
        SetRandomWanderTarget();
        timer = wanderTimer;
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







