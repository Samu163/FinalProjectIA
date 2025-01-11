using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public static FlockManager FM;

    public LeaderMovement leader;

    public GameObject zombiePrefab;
    public int numZombies = 20;
    public GameObject[] allZombies;
    public Transform initialPosition; // Nueva referencia
    public Vector3 flyLimits = new Vector3();

    public Vector3 goalPos = Vector3.zero;

    [Header("Zombie Settings")]
    [Range(0.0f, 5.0f)]
    public float minSpeed;

    [Range(0.0f, 5.0f)]
    public float maxSpeed;

    [Range(1.0f, 10.0f)]
    public float neighbourDistance;

    [Range(1.0f, 5.0f)]
    public float rotationSpeed;

    void Start()
    {
        allZombies = new GameObject[numZombies];

        for (int i = 0; i < numZombies; i++)
        {
            Vector3 pos = initialPosition.position + new Vector3(
                Random.Range(-flyLimits.x, flyLimits.x),
                0, // Ignorar eje Y
                Random.Range(-flyLimits.z, flyLimits.z));

            allZombies[i] = Instantiate(zombiePrefab, pos, Quaternion.identity);
        }

        FM = this;
        goalPos = initialPosition.position;
    }

    void Update()
    {
        if (Random.Range(0, 100) < 10)
        {
            goalPos = initialPosition.position + new Vector3(
                Random.Range(-flyLimits.x, flyLimits.x),
                0,
                Random.Range(-flyLimits.z, flyLimits.z));
        }
    }
}