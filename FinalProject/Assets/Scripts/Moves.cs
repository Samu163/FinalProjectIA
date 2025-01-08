/*  Source: 
    Artificial Intelligence for Beginners
    Penny de Byl
    https://learn.unity.com/course/artificial-intelligence-for-beginners
*/

﻿using UnityEngine;
using UnityEngine.AI;

public class Moves : MonoBehaviour
{
    public GameObject target;
    public Collider floor;
    GameObject[] hidingSpots;
    NavMeshAgent agent;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        hidingSpots = GameObject.FindGameObjectsWithTag("Hide");
    }

    public void Seek(Vector3 location)
    {
        agent.SetDestination(location);
    }

    public void Flee(Vector3 location)
    {
        Vector3 fleeVector = location - this.transform.position;
        agent.SetDestination(this.transform.position - fleeVector);
    }

    public void Pursue()
    {
        Vector3 targetDir = target.transform.position - this.transform.position;

        float relativeHeading = Vector3.Angle(this.transform.forward, this.transform.TransformVector(target.transform.forward));

        float toTarget = Vector3.Angle(this.transform.forward, this.transform.TransformVector(targetDir));

        if ((toTarget > 90 && relativeHeading < 20))
        {
            Seek(target.transform.position);
            return;
        }

        float lookAhead = targetDir.magnitude / (agent.speed);
        Seek(target.transform.position + target.transform.forward * lookAhead);
    }

    public void Evade()
    {
        Vector3 targetDir = target.transform.position - this.transform.position;
        float lookAhead = targetDir.magnitude / agent.speed;
        Flee(target.transform.position + target.transform.forward * lookAhead);
    }


    Vector3 wanderTarget = Vector3.zero;
    public void Wander()
    {
        float wanderRadius = 10;
        float wanderDistance = 10;
        float wanderJitter = 1;

        wanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * wanderJitter,
                                        0,
                                        Random.Range(-1.0f, 1.0f) * wanderJitter);
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
        Vector3 targetWorld = this.gameObject.transform.InverseTransformVector(targetLocal);

        if (!floor.bounds.Contains(targetWorld))
        {
            targetWorld = -transform.position * 0.1f;

        };

        Seek(targetWorld);
    }

    public void Hide()
    {
        float dist = Mathf.Infinity;
        Vector3 chosenSpot = Vector3.zero;
        Vector3 chosenDir = Vector3.zero;
        GameObject chosenGO = null;

        for (int i = 0; i < hidingSpots.Length; i++)
        {
            Vector3 hidingSpotPosition = hidingSpots[i].transform.position;

            float distanceToPolice = Vector3.Distance(hidingSpotPosition, target.transform.position);
            float distanceToThief = Vector3.Distance(hidingSpotPosition, this.transform.position);

            if (distanceToPolice < distanceToThief)
            {
                continue;
            }

            Vector3 hideDir = hidingSpotPosition - target.transform.position;
            Vector3 hidePos = hidingSpotPosition + hideDir.normalized * 100;

            float distanceToHidePos = Vector3.Distance(this.transform.position, hidePos);
            if (distanceToHidePos < dist)
            {
                chosenSpot = hidePos;
                chosenDir = hideDir;
                chosenGO = hidingSpots[i];
                dist = distanceToHidePos;
            }
        }

        if (chosenGO != null)
        {
            Collider hideCol = chosenGO.GetComponent<Collider>();
            Ray backRay = new Ray(chosenSpot, -chosenDir.normalized);
            RaycastHit info;
            float distance = 250.0f;

            if (hideCol.Raycast(backRay, out info, distance))
            {
                Seek(info.point + chosenDir.normalized);
            }
        }
        else
        {
            Debug.LogWarning("No hiding spot found that meets the distance requirements from the police.");
        }
    }

}
