
using UnityEngine;

public class Flock : MonoBehaviour
{
    float speed;
    bool turning = false;
    public Transform leader;


    void Start()
    {
        speed = Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);
        leader = FlockManager.FM.leader.transform;
    }

    void Update()
    {
        Bounds b = new Bounds(FlockManager.FM.initialPosition.position, FlockManager.FM.flyLimits * 2);

        if (!b.Contains(transform.position))
        {
            turning = true;
        }
        else
        {
            turning = false;
        }

        if (turning)
        {
            Vector3 direction = FlockManager.FM.initialPosition.position - transform.position;
            direction.y = 0; // Mantén la dirección horizontal
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), FlockManager.FM.rotationSpeed * Time.deltaTime);
          
        }
        else
        {
            if (Vector3.Distance(transform.position, leader.position) < FlockManager.FM.leader.influenceDistance)
            {
                FollowLeader();
            }
            else
            {
                if (Random.Range(0, 100) < 10)
                {
                    speed = Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);
                }
                if (Random.Range(0, 100) < 10)
                {
                    ApplyFlockingRules();
                }
            }
        }

        transform.Translate(0, 0, speed * Time.deltaTime);
    }

    void FollowLeader()
    {
        if (leader != null)
        {
            Vector3 leaderDirection = leader.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(leaderDirection), FlockManager.FM.rotationSpeed * Time.deltaTime);
            ApplyFlockingRules();
        }
    }

    void ApplyFlockingRules()
    {
        GameObject[] zombies = FlockManager.FM.allZombies;

        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.01f;
        float nDistance;
        int groupSize = 0;

        foreach (GameObject z in zombies)
        {
            if (z != this.gameObject)
            {
                nDistance = Vector3.Distance(z.transform.position, this.transform.position);

                if (nDistance < FlockManager.FM.neighbourDistance)
                {
                    vcentre += z.transform.position;
                    groupSize++;

                    if (nDistance < 1.0f)
                    {
                        vavoid += this.transform.position - z.transform.position;
                    }

                    Flock anotherFlock = z.GetComponent<Flock>();
                    gSpeed += anotherFlock.speed;
                }
            }
        }

        if (groupSize > 0)
        {
            vcentre = vcentre / groupSize + (FlockManager.FM.goalPos - this.transform.position);
            speed = gSpeed / groupSize;

            if (speed > FlockManager.FM.maxSpeed)
            {
                speed = FlockManager.FM.maxSpeed;
            }

            Vector3 direction = (vcentre + vavoid) - this.transform.position;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), FlockManager.FM.rotationSpeed * Time.deltaTime);
            }
        }
    }
}