using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    // Speed of the individual flock (boid/fish)
    float speed;
    
    // Boolean flag to check if the fish needs to turn (when hitting boundaries)
    bool turning = false;
    public Transform leader; 

    
    // Start is called before the first frame update
    // Initializes the fish with a random speed within the bounds set by FlockManager
    void Start()
    {
        // Set initial speed randomly between minimum and maximum speed set in the FlockManager
        speed = Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);
    }

    // Update is called once per frame
    // Handles fish movement, boundary checks, and rule application (cohesion, separation, alignment)
    void Update()
    {
        // Define the swimming area boundary (based on the swimLimits set in FlockManager)
        Bounds b = new Bounds(FlockManager.FM.transform.position, FlockManager.FM.flyLimits * 2);

        // If the fish is outside the boundary, set turning to true to make it turn back
        if (!b.Contains(transform.position))
        {
            turning = true;
        }
        else
        {
            turning = false;
        }

        // If the fish needs to turn (because it hit a boundary)
        if (turning)
        {
            // Calculate the direction towards the center of the swim area
            Vector3 direction = FlockManager.FM.transform.position - transform.position;
            
            // Smoothly rotate towards the center using Slerp (Spherical Linear Interpolation)
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                Quaternion.LookRotation(direction), 
                FlockManager.FM.rotationSpeed * Time.deltaTime);
        }
        else
        {
            if (Vector3.Distance(transform.position, leader.position) <FlockManager.FM.leader.InfluenceDistance)
            {
                FollowLeader();

            }
            else
            {
              
                // Randomly adjust the fish's speed occasionally (10% chance per frame)
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

        // Move the fish forward based on its current speed
        this.transform.Translate(0, 0, speed * Time.deltaTime);
    }
    void FollowLeader()
    {
        if (leader != null)
        {
            Vector3 leaderDirection = leader.position - transform.position;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(leaderDirection),
                FlockManager.FM.rotationSpeed * Time.deltaTime);
            ApplyFlockingRules();
        }
    }
    // ApplyFlockingRules() is responsible for implementing the core flocking behaviors:
    // - Cohesion: Stay near the center of the group.
    // - Separation: Avoid getting too close to other bird.
    // - Alignment: Match the speed of nearby bird.
    void ApplyFlockingRules()
    {
        
        GameObject[] gos = FlockManager.FM.allBird;

       
        Vector3 vcentre = Vector3.zero;  
        Vector3 vavoid = Vector3.zero;  
        float gSpeed = 0.01f;            
        float nDistance;                 
        int groupSize = 0;               

       
        foreach (GameObject go in gos)
        {
           
            if (go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);

                if (nDistance < FlockManager.FM.neighbourDistance)
                {
                    // Cohesion: Add the position of the nearby bird to the center vector

                    vcentre += go.transform.position;
                    groupSize++;
                    // Separation: If the bird is too close, apply an avoidance force

                    if (nDistance < 1.0f)
                    {
                        vavoid += this.transform.position - go.transform.position;
                    }
                    // Alignment: Match speed with nearby birds

                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed += anotherFlock.speed;
                }
            }
        }

        
        if (groupSize > 0)
        {
            // Cohesion: Calculate the average position of the group and move toward it

            vcentre = vcentre / groupSize + (FlockManager.FM.goalPos - this.transform.position);
            speed = gSpeed / groupSize;
            // Alignment: Set the speed to the average speed of the group

            if (speed > FlockManager.FM.maxSpeed)
            {
                speed = FlockManager.FM.maxSpeed;
            }
            
            Vector3 direction = (vcentre + vavoid) - this.transform.position;
           
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, 
                    Quaternion.LookRotation(direction), 
                    FlockManager.FM.rotationSpeed * Time.deltaTime);
            }
        }
    }
}