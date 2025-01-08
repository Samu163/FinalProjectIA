using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderMovement : MonoBehaviour
{
    public float speed = 3f; 
    public float rotationSpeed = 2f; 
    public float changeTargetInterval = 2f; 

    private Vector3 targetPosition; 
    private float boundaryX = 10f; 
    private float boundaryY = 5f; 
    private float boundaryZ = 10f; 
    public float InfluenceDistance = 5f;

    void Start()
    {
        SetNewTargetPosition();
        StartCoroutine(ChangeTargetPositionCoroutine());
    }

    void Update()
    {
        MoveTowardsTarget();
    }

    void MoveTowardsTarget()
    {
        // Calculate the direction towards the target position
        Vector3 direction = targetPosition - transform.position;

        // If it is not close to the target position
        if (direction.magnitude > 0.1f)
        {
            // Rotate towards the target position
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move forward
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    void SetNewTargetPosition()
    {
        // Generate a new random position within the boundaries
        float randomX = Random.Range(-boundaryX, boundaryX);
        float randomY = Random.Range(-boundaryY, boundaryY); 
        float randomZ = Random.Range(-boundaryZ, boundaryZ);
        targetPosition = new Vector3(randomX, randomY, randomZ);
    }

    IEnumerator ChangeTargetPositionCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(changeTargetInterval);
            SetNewTargetPosition(); // Change to a new target position
        }
    }
}

