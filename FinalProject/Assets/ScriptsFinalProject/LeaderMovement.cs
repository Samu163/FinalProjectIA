using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderMovement : MonoBehaviour
{
    public float speed = 3f;
    public float rotationSpeed = 2f;
    public float changeTargetInterval = 2f;

    private Vector3 targetPosition;
    public Transform initialPosition; // Nueva referencia para ajustar límites
    public float boundaryX = 10f;
    public float boundaryZ = 10f;
    public float influenceDistance = 5f;

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
        Vector3 direction = targetPosition - transform.position;
        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    void SetNewTargetPosition()
    {
        float randomX = Random.Range(-boundaryX, boundaryX) + initialPosition.position.x;
        float randomZ = Random.Range(-boundaryZ, boundaryZ) + initialPosition.position.z;
        targetPosition = new Vector3(randomX, transform.position.y, randomZ); // Solo XZ
    }

    IEnumerator ChangeTargetPositionCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(changeTargetInterval);
            SetNewTargetPosition();
        }
    }
}