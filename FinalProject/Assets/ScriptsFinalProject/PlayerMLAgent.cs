using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PlayerMLAgent : Agent
{
    public Transform target; 
    public Transform[] policemans; 
    public float speed = 2f;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        target.localPosition = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
    }

    public override void CollectObservations(VectorSensor sensor)
    {       
        sensor.AddObservation(transform.localPosition - target.localPosition);

        foreach (var policia in policemans)
        {
            sensor.AddObservation(transform.localPosition - policia.localPosition);
        }

        sensor.AddObservation(transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var Xmovement = actions.ContinuousActions[0];
        var ZMovement = actions.ContinuousActions[1];
        transform.localPosition += new Vector3(Xmovement, 0, ZMovement) * speed * Time.deltaTime;

        float distanciaObjetivo = Vector3.Distance(transform.localPosition, target.localPosition);

        if (distanciaObjetivo < 1.0f)
        {
            SetReward(1.0f); 
            EndEpisode();
        }

        foreach (var policia in policemans)
        {
            if (Vector3.Distance(transform.localPosition, policia.localPosition) < 1.0f)
            {
                SetReward(-1.0f); 
                EndEpisode();
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }
}

