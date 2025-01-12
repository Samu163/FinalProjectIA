using UnityEngine;

public class Flock : MonoBehaviour
{
    float speed;
    bool turning = false;

    // El líder asignado desde FlockManager
    private Transform leader;

    void Start()
    {
        speed = Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);
    }

    void Update()
    {
        // Asignar líder desde FlockManager
        if (FlockManager.FM.leader != null)
        {
            leader = FlockManager.FM.leader.transform;
        }
        else
        {
            Debug.LogWarning("Leader not assigned in FlockManager.");
            return;
        }

        // Límite de movimiento del FlockManager
        Bounds b = new Bounds(FlockManager.FM.initialPosition.position, FlockManager.FM.flyLimits * 2);

        // Detectar si está fuera de los límites
        turning = !b.Contains(transform.position);

        if (turning)
        {
            MoveBackToBounds();
        }
        else
        {
            // Seguir al líder si está dentro de la distancia de influencia
            if (Vector3.Distance(transform.position, leader.position) < FlockManager.FM.leader.influenceDistance)
            {
                FollowLeader();
            }
            else
            {
                ApplyFlockingRules();
            }

            // Cambiar la velocidad aleatoriamente
            if (Random.Range(0, 100) < 10)
            {
                speed = Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);
            }
        }

        // Movimiento final
        transform.Translate(0, 0, speed * Time.deltaTime);
    }

    void MoveBackToBounds()
    {
        Vector3 direction = FlockManager.FM.initialPosition.position - transform.position;
        direction.y = 0; // Mantener en el plano horizontal
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), FlockManager.FM.rotationSpeed * Time.deltaTime);
    }

    void FollowLeader()
    {
        if (leader != null)
        {
            Vector3 directionToLeader = leader.position - transform.position;
            directionToLeader.y = 0; // Mantener en el plano horizontal
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToLeader), FlockManager.FM.rotationSpeed * Time.deltaTime);

            // Ajustar la velocidad al acercarse al líder
            speed = Mathf.Lerp(speed, FlockManager.FM.maxSpeed, Time.deltaTime);
        }
    }

    void ApplyFlockingRules()
    {
        GameObject[] zombies = FlockManager.FM.allZombies;

        Vector3 center = Vector3.zero;
        Vector3 avoid = Vector3.zero;
        float groupSpeed = 0f;
        int groupSize = 0;

        foreach (GameObject zombie in zombies)
        {
            if (zombie != this.gameObject)
            {
                float distance = Vector3.Distance(zombie.transform.position, this.transform.position);

                if (distance < FlockManager.FM.neighbourDistance)
                {
                    center += zombie.transform.position;
                    groupSize++;

                    if (distance < 1.0f)
                    {
                        avoid += this.transform.position - zombie.transform.position;
                    }

                    Flock otherFlock = zombie.GetComponent<Flock>();
                    if (otherFlock != null)
                    {
                        groupSpeed += otherFlock.speed;
                    }
                }
            }
        }

        if (groupSize > 0)
        {
            center = center / groupSize + (FlockManager.FM.goalPos - this.transform.position);
            speed = Mathf.Clamp(groupSpeed / groupSize, FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);

            Vector3 direction = (center + avoid) - transform.position;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), FlockManager.FM.rotationSpeed * Time.deltaTime);
            }
        }
    }
}
