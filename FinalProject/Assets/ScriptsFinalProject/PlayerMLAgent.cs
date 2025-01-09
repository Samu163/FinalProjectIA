using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PlayerMLAgent : Agent
{
    public Transform[] targets; // Array de objetivos
    public Transform[] policemans; // Array de policías
    public float speed = 2f;
    private int collectedTargets = 0;

    public override void OnEpisodeBegin()
    {
        // Reinicia la posición del ladrón
        transform.localPosition = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));

      GameManager.Instance.boxInstantiator.InstantiateBoxes(); // Genera 5 cajas

        // Reinicia el contador de objetivos recogidos
        collectedTargets = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observa la posición relativa a cada objetivo
        foreach (var target in targets)
        {
            if (target.gameObject.activeSelf)
            {
                sensor.AddObservation(transform.localPosition - target.localPosition);
            }
            else
            {
                // Si el objetivo está inactivo, agrega una observación nula
                sensor.AddObservation(Vector3.zero);
            }
        }

        // Observa la posición relativa de los policías
        foreach (var police in policemans)
        {
            sensor.AddObservation(transform.localPosition - police.localPosition);
        }

        // Observa la posición actual del ladrón
        sensor.AddObservation(transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Mueve el ladrón según las acciones
        var Xmovement = actions.ContinuousActions[0];
        var ZMovement = actions.ContinuousActions[1];
        transform.localPosition += new Vector3(Xmovement, 0, ZMovement) * speed * Time.deltaTime;

        // Revisa si se alcanza algún objetivo
        foreach (var target in targets)
        {
            if (target.gameObject.activeSelf && Vector3.Distance(transform.localPosition, target.localPosition) < 1.0f)
            {
                target.gameObject.SetActive(false); // Desactiva el objetivo
                collectedTargets++;
                SetReward(1.0f); // Recompensa por alcanzar un objetivo
            }
        }

        // Revisa si el agente es atrapado por un policía
        foreach (var police in policemans)
        {
            if (Vector3.Distance(transform.localPosition, police.localPosition) < 1.0f)
            {
                SetReward(-1.0f); // Penalización por ser atrapado
                EndEpisode();
            }
        }

        // Finaliza el episodio si todos los objetivos han sido recogidos
        if (collectedTargets == targets.Length)
        {
            SetReward(2.0f); // Recompensa adicional por completar todos los objetivos
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Controles manuales para prueba
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }
}

