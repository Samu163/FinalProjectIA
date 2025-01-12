using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{
    Rigidbody rBody;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public Transform Target;

    public override void OnEpisodeBegin()
    {
        // Reiniciar la velocidad y posición del agente al comienzo del episodio
        rBody.angularVelocity = Vector3.zero;
        rBody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(0, 0.5f, 0);

        // Mover el objetivo a una nueva posición aleatoria
        Target.localPosition = new Vector3(
            Random.value * 8 - 4,
            0.5f,
            Random.value * 8 - 4
        );
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observaciones: posiciones del objetivo y del agente
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Velocidad del agente
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    public float forceMultiplier = 10;

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Acciones, tamaño = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        // Calcular la distancia al objetivo
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Recompensa por alcanzar el objetivo
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            GameManager.Instance.AddZombiePoint();
            EndEpisode();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Terminar episodio si choca con un obstáculo
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            SetReward(-1.0f); // Penalización por chocar con un obstáculo
            EndEpisode();
        }
    }
}
