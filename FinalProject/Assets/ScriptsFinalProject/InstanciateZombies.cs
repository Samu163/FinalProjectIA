using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public int numberOfZombies = 10;
    public float spawnRadius = 20f;
    public List<GameObject> zombies = new List<GameObject>();

    void Start()
    {
        //SpawnZombies();
    }

    public void SpawnZombies()
    {
        for (int i = 0; i < numberOfZombies; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();

            if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                var zombie = Instantiate(zombiePrefab, hit.position, Quaternion.identity);
                zombies.Add(zombie);
            }
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection += transform.position;

        randomDirection.y = 0;

        return randomDirection;
    }
}
